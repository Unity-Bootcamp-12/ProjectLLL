using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAction : NetworkBehaviour
{
    [SerializeField] private Animator _modelAnimator;
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
        
    }

    private void Update()
    {
        TestRpc();

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    SetAnimatorBoolRpc("IsDead", false);
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    _navMeshAgent.isStopped = true;
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    SetAnimatorBoolRpc("IsRun", true);
        //    SetAnimatorBoolRpc("IsAttack", false);
        //    _navMeshAgent.isStopped = false;
        //}





        if (IsHost)
        {
            SetAnimatorBoolRpc("IsRun", _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance);
        }

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
                AnimatorStateInfo stateInfo = _modelAnimator.GetCurrentAnimatorStateInfo(0);
                if (distanceToTarget < _playerController.GetAttackRange())
                {
                    SetAnimatorBoolRpc("IsRun", false);
                    SetAnimatorBoolRpc("IsAttack", true);
                    if (stateInfo.IsName("IsAttack")) { _navMeshAgent.isStopped = true; }
                    StopMove();
                    _attackCoroutine = StartCoroutine(AttackCoroutine(1.0f, 1.0f));
                    
                }
                else
                {
                    SetAnimatorBoolRpc("IsRun", true);
                    SetAnimatorBoolRpc("IsAttack", false);
                    SetMoveDestinationRpc(_target.transform.position);
                }
            }
        }

        if (distanceToTarget > MOVE_STOPPING_DISTANCE)
        {
            SetAnimatorBoolRpc("IsRun", true);
            SetAnimatorBoolRpc("IsAttack", false);
            SetMoveDestinationRpc(_target.transform.position);
            
            //if (stateInfo.IsName("IsAttack")) { _navMeshAgent.isStopped = true; }
        }
    }

    [Rpc(SendTo.Server)]
    private void TestRpc()
    {
        TestTestRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void TestTestRpc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetAnimatorBoolRpc("IsDead", false);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _navMeshAgent.isStopped = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetAnimatorBoolRpc("IsRun", true);
            SetAnimatorBoolRpc("IsAttack", false);
            _navMeshAgent.isStopped = false;
        }
    }


    [Rpc(SendTo.Server)]
    private void SetAnimatorBoolRpc(string name, bool param)
    {
        _modelAnimator.SetBool(name, param);
    }

    public void StopMove()
    {
        _target = null;
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        SetAnimatorBoolRpc("IsRun", false);
    }

    public void Dead()
    {
        SetAnimatorBoolRpc("IsDead", true);
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
        SetAnimatorBoolRpc("IsAttack", false);
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
                transform.LookAt(hit.transform.position);
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
        if (_playerController.GetAttackType() == AttackType.Melee)
        {
            Logger.Info($"Melee Attack: {unitController.name}");
            unitController.ReceiveDamage(_playerController.GetAttackPower());
            
        }
        else if (_playerController.GetAttackType() == AttackType.Ranged)
        {
            Logger.Info($"Ranged Attack: {unitController.name}");
            FireTargetProjectileRpc(unitController.NetworkObjectId, 2.0f, _playerController.GetAttackPower());

        }
    }

    [Rpc(SendTo.Server)]
    public void FireTargetProjectileRpc(ulong targetId, float speed, float damage)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetId, out var targetNetworkObject))
        {
            return;
        }

        GameObject projectileObject = Instantiate(_playerController.GetProjectilePrefab(), transform.position, Quaternion.identity);
        projectileObject.GetComponent<TargetProjectile>().Init(targetNetworkObject.transform, speed, damage);
    }
}