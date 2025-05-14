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
            if (_playerController.TeamType != _target.TeamType)
            {
                if (Vector3.Distance(transform.position, _target.transform.position) <= _playerController.GetAttackRange())
                {
                    if (_isAttacking != true)
                    {
                        GameManager.Instance.PlayAfterCoroutine(() =>
                        {
                            _isAttacking = true;

                            Attack(_target);

                            GameManager.Instance.PlayAfterCoroutine(() =>
                            {
                                _isAttacking = false;
                            }, _playerController.GetAttackSpeed());

                        }, _playerController.GetAttackSpeed());

                        return;
                    }
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
        unitController.ReceiveDamage(unitController.GetAttackPower());
    }
}