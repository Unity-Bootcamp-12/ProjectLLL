using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;

    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _navMeshAgent.acceleration = 1000f;
        _navMeshAgent.angularSpeed = 720f;
        _navMeshAgent.stoppingDistance = 0f;
        _navMeshAgent.autoBraking = false; // 시도해보고 미끄러지면 꺼주세요
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, _groundMask))
            {
                _navMeshAgent.SetDestination(hit.point);
            }
        }
    }
}
