using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class MinionController : UnitController
{
    const float MOVE_STOPPING_DISTANCE = 3.0f;

    private Vector3 _moveDestination;

    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Init(UnitTeamType team, Vector3 destination)
    {
        NetworkObject.Spawn();
        SetTeamTypeRpc(team);
        _moveDestination = destination;
        SetMoveDestinationRpc(destination);
    }

    public override void Dead()
    {
        NetworkObject.Despawn();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (_target == null)
        {
            if (!_isAttacking)
            {
                SetMoveDestinationRpc(_moveDestination);
                FindUnitInRange();
            }
            return;
        }

        if (_target.IsDead || !IsTargetInAttackDetectRange())
        {
            _target = null;
            return;
        }

        if (IsTargetInAttackRange())
        {
            if (_isAttacking)
            {
                return;
            }
            else
            {
                _attackCoroutine = StartCoroutine(AttackCoroutine(_target, 1.0f, 1.0f));
                StopMove();
            }
        }
    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHPRpc(-damage);
    }
}
