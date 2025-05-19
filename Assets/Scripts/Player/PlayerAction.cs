using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAction : NetworkBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _unitMask;

    private PlayerController _playerController;
    private NavMeshAgent _navMeshAgent;

    const float MOVE_STOPPING_DISTANCE = 3.0f;

    private UnitController _target;
    private Coroutine _attackCoroutine = null;

    private bool _isAttacking = false;
    private bool _isPreAttacking = false;
    private bool _isPostAttacking = false;
    //private float _nextAttackTime = 0.0f;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _navMeshAgent.acceleration = 1000f;
        _navMeshAgent.angularSpeed = 720f;
        _navMeshAgent.stoppingDistance = 0.1f;
        _navMeshAgent.autoBraking = false;

        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightMouseDown);
        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftMouseDown);

        // TEST

        if (IsLocalPlayer)
        {
            if (IsHost)
            {
                _attackType = AttackType.Melee;
            }
            else
            {
                _attackType = AttackType.Ranged;
            }
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (_target == null)
        {
            return;
        }

        if (_target.IsDead)
        {
            _target = null;
            return;
        }

        //Logger.Info($"{_target.name}");

        float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
        if (_playerController.TeamType != _target.TeamType)
        {
            if (_isAttacking)
            {
                return;
            }
            else
            {
                if (distanceToTarget < _playerController.GetAttackRange())
                {
                    _attackCoroutine = StartCoroutine(AttackCoroutine(1.0f, 1.0f));
                }
                else
                {
                    SetMoveDestinationRpc(_target.transform.position);
                }
            }
        }

        if (distanceToTarget > MOVE_STOPPING_DISTANCE)
        {
            SetMoveDestinationRpc(_target.transform.position);
        }
    }

    public void StopMove()
    {
        _target = null;
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
    }

    public void StopAttack()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }
        _isAttacking = false;
        _isPreAttacking = false;
        _isPostAttacking = false;
    }

    private IEnumerator AttackCoroutine(float preAttackDelayTime, float postAttackDelayTime)
    {
        _isAttacking = true;
        _isPreAttacking = true;

        UnitController attackTarget = _target;
        Logger.Info("Attack Start");

        yield return new WaitForSeconds(preAttackDelayTime);

        Logger.Info("Attack");
        Attack(attackTarget);

        _isPostAttacking = false;
        yield return new WaitForSeconds(postAttackDelayTime);

        _isAttacking = false;
        _isPreAttacking = false;
        _isPostAttacking = false;
        Logger.Info("Attack End");
    }

    public void OnLeftMouseDown()
    {
        if (!IsOwner)
        {
            return;
        }

        if (_playerController.IsDead)
        {
            Logger.Info("IsDead 켜져있음");
            return;
        }

        if (_playerController.IsAttackButtonDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit unitHit, 100f, _unitMask))
            {
                GameObject hitObject = unitHit.collider.gameObject;

                if (hitObject.TryGetComponent<UnitController>(out var unit))
                {
                    if (unit.TeamType != _playerController.TeamType)
                    {
                        _target = unit;
                    }
                }
            }
            else if (Physics.Raycast(ray, out RaycastHit hit, 100f, _groundMask))
            {
                SetMoveDestinationRpc(hit.point);
            }
        }
    }

    public void OnRightMouseDown()
    {
        if (!IsOwner)
        {
            return;
        }

        if (_playerController.IsDead)
        {
            Logger.Info("IsDead 켜져있음");
            return;
        }

        _target = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit unitHit, 100f, _unitMask))
        {
            GameObject hitObject = unitHit.collider.gameObject;

            if (hitObject.TryGetComponent<UnitController>(out var unit))
            {
                Logger.Info($"Mouse Hit Unit: {unit.name}");
                _target = unit;
            }
        }
        else if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, _groundMask))
        {
            //_navMeshAgent.SetDestination(groundHit.point);
            SetMoveDestinationRpc(groundHit.point);
        }
    }

    [Rpc(SendTo.Server)]
    private void SetMoveDestinationRpc(Vector3 target)
    {
        _navMeshAgent.SetDestination(target);
    }

    public void Attack(UnitController unitController)
    {
        if (_attackType == AttackType.Melee)
        {
            Logger.Info($"Melee Attack: {unitController.name}");
            unitController.ReceiveDamage(_playerController.GetAttackPower());
        }
        else if (_attackType == AttackType.Ranged)
        {
            Logger.Info($"Ranged Attack: {unitController.name}");
            FireTargetProjectileRpc(unitController.NetworkObjectId);
        }
    }

    // ----------- TEST -------------------------
    [SerializeField] private GameObject projectilePrefab;

    [Rpc(SendTo.Server)]
    public void FireTargetProjectileRpc(ulong targetId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetId, out var targetNetworkObject))
        { 
            return;
        }

        GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectileObject.GetComponent<TargetProjectile>().Init(targetNetworkObject.transform, 10f, 25f);
    }

    [SerializeField] private AttackType _attackType;

    private enum AttackType
    {
        Melee,   // 근거리
        Ranged   // 원거리
    }
}