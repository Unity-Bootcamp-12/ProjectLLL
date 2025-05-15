using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAction : MonoBehaviour
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
        _navMeshAgent.stoppingDistance = 0f;
        _navMeshAgent.autoBraking = false;

        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightMouseDown);
        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftMouseDown);
    }

    private void Update()
    {
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
                    _navMeshAgent.SetDestination(_target.transform.position);
                }
            }
        }

        if (distanceToTarget > MOVE_STOPPING_DISTANCE)
        {
            _navMeshAgent.SetDestination(_target.transform.position);
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
        Logger.Info("Attack Start");

        yield return new WaitForSeconds(preAttackDelayTime);

        Logger.Info("Attack");
        Attack(_target);

        _isPostAttacking = false;
        yield return new WaitForSeconds(postAttackDelayTime);

        _isAttacking = false;
        _isPreAttacking = false;
        _isPostAttacking = false;
        Logger.Info("Attack End");
    }

    public void OnLeftMouseDown()
    {
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
                _navMeshAgent.SetDestination(hit.point);
            }
        }
    }

    public void OnRightMouseDown()
    {
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
            _navMeshAgent.SetDestination(groundHit.point);
        }
    }

    public void Attack(UnitController unitController)
    {
        unitController.ReceiveDamage(_playerController.GetAttackPower());
    }
}