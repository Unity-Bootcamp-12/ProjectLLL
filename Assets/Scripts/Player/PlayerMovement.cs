using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _unitMask;

    private PlayerController _playerController;
    private NavMeshAgent _navMeshAgent;

    const float MOVE_STOPPING_DISTANCE = 3.0f;

    private UnitController _target;
    private bool _isAttacking = false;
    private float _nextAttackTime = 0.0f;

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
        if (_target != null)
        {
            if (_target.IsDead)
            {
                _target = null;
                return;
            }
            Logger.Info($"{_target.name}");
            if (_playerController.TeamType != _target.TeamType)
            {
                if (Vector3.Distance(transform.position, _target.transform.position) <= _playerController.GetAttackRange()
                    && Time.time >= _nextAttackTime)
                {
                    GameManager.Instance.PlayAfterCoroutine(() =>
                    {
                        if (!_isAttacking)
                        {
                            _isAttacking = true;
                            Attack(_target);
                            Logger.Info("공격 진행 중");

                            GameManager.Instance.PlayAfterCoroutine(() =>
                            {
                                _isAttacking = false;
                                Logger.Info($"{_isAttacking}");

                                _nextAttackTime = Time.time + 3.0f;
                            }, 3.0f);
                        }
                    }, 3.0f);
                }
            }

            if (Vector3.Distance(transform.position, _target.transform.position) > MOVE_STOPPING_DISTANCE)
            {
                _navMeshAgent.SetDestination(_target.transform.position);
            }
        }
    }

    public void StopMove()
    {
        _target = null;
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
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