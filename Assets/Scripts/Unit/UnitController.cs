using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 챔피언, 미니언, 타워 등 HP를 가진 유닛의 Base 클래스
/// </summary>
public abstract class UnitController : NetworkBehaviour
{
    [SerializeField] protected LayerMask _unitLayerMask;
    [SerializeField] private Animator _modelAnimator;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private bool _isPlayer = false; // 파티클 출력을 위해 플레이어인지 미니언인지 구분하기 위함...

    [SerializeField] private Outline _modelOutline;

    //여기에 있으면 안되긴함... 팩토리가 존재해야함
    [SerializeField] private GameObject _nonTargetProjectilePrefab;

    [SerializeField] protected UnitHPBarUI _unitHPBarUI;

    protected const float MOVE_STOPPING_DISTANCE = 1.5f;

    protected Collider _collider;
    protected HPController _hpController;
    protected UnitStatusController _unitStatusController;
    protected NavMeshAgent _navMeshAgent;

    public UnitTeamType TeamType => _teamType.Value;
    [SerializeField] protected NetworkVariable<UnitTeamType> _teamType;

    public NetworkVariable<bool> IsDead { get; protected set; }
    [SerializeField] protected UnitController _target;
    protected Coroutine _attackCoroutine = null;

    protected bool _isAttacking = false;
    protected bool _isPreAttacking = false;
    protected bool _isPostAttacking = false;

    protected float _attackDetectRange;

    [Rpc(SendTo.Server)]
    public void SetTeamTypeRpc(UnitTeamType teamType)
    {
        _teamType.Value = teamType;
    }

    public float GetAttackPower()
    {
        return _unitStatusController.GetAttackPower();
    }

    public float GetAttackRange()
    {
        return _unitStatusController.GetAttackRange();
    }

    public float GetAttackSpeed()
    {
        return _unitStatusController.GetAttackSpeed();
    }

    public AttackType GetAttackType()
    {
        if (IsLocalPlayer)
        {
            if (IsHost)
            {
                return AttackType.Melee;
            }
            else
            {
                return AttackType.Ranged;
            }
        }

        return _unitStatusController.GetAttackType();
    }

    public GameObject GetProjectilePrefab()
    {
        return _unitStatusController.GetProjectilePrefab();
    }


    public string GetHeroName()
    {
        return _unitStatusController.GetHeroName();
    }

    public abstract void ReceiveDamage(float damage);
    public abstract void Dead();

    protected virtual void Awake()
    {
        _hpController = GetComponent<HPController>();
        _unitStatusController = GetComponent<UnitStatusController>();
        _collider = GetComponent<Collider>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        SetOutline(false);

        IsDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }

    public virtual void Init(UnitTeamType team, ulong clientId)
    {
        InitUnitUIRpc(team);
        ApplyChangedStatus();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void InitUnitUIRpc(UnitTeamType teamType)
    {
        _hpController.Init(_unitStatusController.GetMaxHP());
        _hpController.OnDeadEvent.AddListener(Dead);

        _unitHPBarUI.Init(_hpController.OnChangeHPEvent);
        _unitHPBarUI.SetTeam(teamType);
    }

    [Rpc(SendTo.Server)]
    protected void SetMoveDestinationRpc(Vector3 target)
    {
        _navMeshAgent.SetDestination(target);
        SetAnimatorBoolRpc("IsRun", true);
    }

    protected void Attack(UnitController unitController)
    {
        if (GetAttackType() == AttackType.Melee)
        {
            unitController.ReceiveDamage(GetAttackPower());
            if (_isPlayer == true)
            {
                ParticleManager.Instance.PlayParticleServerRpc(ParticleType.PlayerHit, unitController.transform.position);
            }
        }
        else if (GetAttackType() == AttackType.Ranged)
        {
            FireTargetProjectileRpc(unitController.NetworkObjectId, 12.0f, GetAttackPower());
        }
    }

    [Rpc(SendTo.Server)]
    protected void FireTargetProjectileRpc(ulong targetId, float speed, float damage)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetId, out var targetNetworkObject))
        {
            return;
        }

        GameObject projectileObject = Instantiate(GetProjectilePrefab(), _projectileSpawnPoint.position, Quaternion.identity);
        projectileObject.GetComponent<TargetProjectile>().Init(targetNetworkObject.transform, speed, damage);
    }

    [Rpc(SendTo.Server)]
    public void FireNonTargetProjectileRpc(Vector3 destination, float speed, float damage, float lifeTime)
    {
        GameObject projectileObject = Instantiate(_nonTargetProjectilePrefab, _projectileSpawnPoint.position, Quaternion.identity);
        projectileObject.GetComponent<NonTargetProjectile>().Init(destination, speed, damage, lifeTime, TeamType);
    }

    protected IEnumerator AttackCoroutine(UnitController attackTarget, float preAttackDelayTime, float postAttackDelayTime)
    {
        SetAnimatorBoolRpc("IsAttack", true);
        _isAttacking = true;
        _isPreAttacking = true;

        yield return new WaitForSeconds(preAttackDelayTime);

        if (attackTarget != null)
        {
            Attack(attackTarget);
        }

        _isPostAttacking = true;
        yield return new WaitForSeconds(postAttackDelayTime);

        _isAttacking = false;
        _isPreAttacking = false;
        _isPostAttacking = false;
    }

    protected void StopAttack()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }

        _isAttacking = false;
        _isPreAttacking = false;
        _isPostAttacking = false;

        SetAnimatorBoolRpc("IsAttack", false);
    }

    [Rpc(SendTo.Server)]
    protected void StopMoveRpc()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();

        SetAnimatorBoolRpc("IsMove", false);
    }

    [Rpc(SendTo.Server)]
    protected void LookAtRpc(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
    }

    [Rpc(SendTo.Server)]
    protected void FindUnitInRangeRpc()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _attackDetectRange, _unitLayerMask);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<UnitController>(out var unit))
            {
                if (unit.TeamType == TeamType || unit.IsDead.Value)
                {
                    continue;
                }

                SetTargetByIdRpc(unit.NetworkObjectId);
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected void SetTargetByIdRpc(ulong targetNetworkObjectId)
    {
        if (!IsOwner)
        {
            return;
        }

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkObjectId, out var netObject))
        {
            if (netObject.TryGetComponent<UnitController>(out var unit))
            {
                _target = unit;
            }
        }
    }

    protected bool IsTargetInAttackDetectRange()
    {
        Vector3 selfPos = transform.position;
        Vector3 targetPos = _target.transform.position;

        selfPos.y = 0;
        targetPos.y = 0;

        return Vector3.Distance(selfPos, targetPos) < _attackDetectRange;
    }

    protected bool IsTargetInAttackRange()
    {
        Vector3 selfPos = transform.position;
        Vector3 targetPos = _target.transform.position;

        selfPos.y = 0;
        targetPos.y = 0;

        return Vector3.Distance(selfPos, targetPos) < GetAttackRange();
    }

    [Rpc(SendTo.Server)]
    protected void SetAnimatorBoolRpc(string name, bool param)
    {
        _modelAnimator.SetBool(name, param);
    }

    [Rpc(SendTo.Server)]
    protected void SetAnimatorTriggerRpc(string name)
    {
        _modelAnimator.SetTrigger(name);
    }

    [Rpc(SendTo.Server)]
    private void SetMoveSpeedRpc(float moveSpeed)
    {
        if (_navMeshAgent != null)
        {
            _navMeshAgent.speed = moveSpeed;
        }
    }

    protected void ApplyChangedStatus()
    {
        _hpController.SetMaxHPRpc(_unitStatusController.GetMaxHP());
        SetMoveSpeedRpc(_unitStatusController.GetMoveSpeed());
    }

    public void SetOutline(bool isActive)
    {
        if (_modelOutline != null)
        {
            _modelOutline.enabled = isActive;
        }
    }

    // 에디터 전용 Gizmo 코드
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackDetectRange);
    }
}

[Serializable]
public enum UnitTeamType
{
    None,
    RedTeam,
    BlueTeam,
}