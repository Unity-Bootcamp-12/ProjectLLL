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

    [SerializeField] protected UnitHPBarUI _unitHPBarUI;

    protected Collider _collider;
    protected HPController _hpController;
    protected UnitStatusController _unitStatusController;
    protected NavMeshAgent _navMeshAgent;

    public UnitTeamType TeamType => _teamType.Value;
    [SerializeField] private NetworkVariable<UnitTeamType> _teamType;

    public bool IsDead { get; protected set; }
    [SerializeField] protected UnitController _target;
    protected Coroutine _attackCoroutine = null;

    protected bool _isAttacking = false;
    protected bool _isPreAttacking = false;
    protected bool _isPostAttacking = false;

    protected float _attackDetectRange => GetAttackRange() * 2.0f;

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

    public int GetLevel()
    {
        return _unitStatusController.GetLevel();
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
    }

    protected virtual void Start()
    {
        _hpController.Init(_unitStatusController.GetMaxHP());
        _hpController.OnDeadEvent.AddListener(Dead);
        _unitHPBarUI.Init(_hpController.OnChangeHPEvent);
    }

    [Rpc(SendTo.Server)]
    protected void SetMoveDestinationRpc(Vector3 target)
    {
        _navMeshAgent.SetDestination(target);
    }

    protected void Attack(UnitController unitController)
    {
        if (GetAttackType() == AttackType.Melee)
        {
            unitController.ReceiveDamage(GetAttackPower());
        }
        else if (GetAttackType() == AttackType.Ranged)
        {
            FireTargetProjectileRpc(unitController.NetworkObjectId, 2.0f, GetAttackPower());
        }
    }

    [Rpc(SendTo.Server)]
    protected void FireTargetProjectileRpc(ulong targetId, float speed, float damage)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetId, out var targetNetworkObject))
        {
            return;
        }

        GameObject projectileObject = Instantiate(GetProjectilePrefab(), transform.position, Quaternion.identity);
        projectileObject.GetComponent<TargetProjectile>().Init(targetNetworkObject.transform, speed, damage);
    }

    protected IEnumerator AttackCoroutine(UnitController attackTarget, float preAttackDelayTime, float postAttackDelayTime)
    {
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
    }

    protected void StopMove()
    {
        _target = null;
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
    }

    protected void FindUnitInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _attackDetectRange, _unitLayerMask);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<UnitController>(out var unit))
            {
                if (unit.TeamType == TeamType)
                {
                    continue;
                }

                _target = unit;
            }
        }
    }

    protected bool IsTargetInAttackDetectRange()
    {
        return Vector3.Distance(transform.position, _target.transform.position) < _attackDetectRange;
    }

    protected bool IsTargetInAttackRange()
    {
        return Vector3.Distance(transform.position, _target.transform.position) < GetAttackRange();
    }
}

[Serializable]
public enum UnitTeamType
{
    RedTeam,
    BlueTeam,
}