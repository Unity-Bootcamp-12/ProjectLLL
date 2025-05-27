using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class MinionController : UnitController
{
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

        _attackDetectRange = Mathf.Clamp(GetAttackRange() * 2.0f, 6.0f, 10.0f);
    }

    public override void Dead()
    {
        NetworkObject.Despawn();
        IsDead.Value = true;

        if (Random.value < 0.33f)
        {
            GameManager.Instance.SpawnItemRpc(transform.position + Vector3.up);
        }
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
                FindUnitInRangeRpc();
            }
            return;
        }

        if (_target.IsDead.Value || !IsTargetInAttackDetectRange())
        {
            _target = null;
            return;
        }

        if (IsTargetInAttackRange())
        {
            if (_isAttacking)
            {
                LookAtRpc(_target.transform.position);
                return;
            }
            else
            {
                _attackCoroutine = StartCoroutine(AttackCoroutine(_target, 1.0f, 1.0f));
                StopMoveRpc();
            }
        }
    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHPRpc(-damage);
    }
}
