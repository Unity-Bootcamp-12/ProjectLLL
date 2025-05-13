using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _unitMask;

    private PlayerController _playerController;
    private NavMeshAgent _navMeshAgent;

    const float _moveStoppingDistance = 3.0f;

    private Transform _target;

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
        _navMeshAgent.autoBraking = false; // 시도해보고 미끄러지면 꺼주세요

        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightMouseDown);
    }

    private void Update()
    {
        if (_target != null)
        {
            if (Vector3.Distance(transform.position, _target.position) > _moveStoppingDistance)
            {
                _navMeshAgent.SetDestination(_target.position);
            }
        }
    }

    public void StopMove()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
    }

    public void OnLeftMouseDown()
    {
        if (_playerController.IsAttackButtonDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit unitHit, 100f, _unitMask))
            {
                GameObject hitObject = unitHit.collider.gameObject;

                if (hitObject.TryGetComponent<UnitController>(out var unit))
                {
                    Debug.Log($"유닛 감지: {hitObject.name}");
                    if (unit.TeamType != _playerController.TeamType)
                    {
                        _target = hitObject.transform;
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
        _target = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit unitHit, 100f, _unitMask))
        {
            GameObject hitObject = unitHit.collider.gameObject;

            if (hitObject.TryGetComponent<UnitController>(out var unit))
            {
                Debug.Log($"유닛 감지: {hitObject.name}");
                _target = hitObject.transform;
            }
        }
        else if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, _groundMask))
        {
            _navMeshAgent.SetDestination(groundHit.point);
        }
    }
}