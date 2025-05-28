using Unity.Netcode;

public class TowerController : UnitController
{
    public override void Init(UnitTeamType team, ulong clientId)
    {
        NetworkObject.Spawn();
        
        base.Init(team, clientId);

        SetTeamTypeRpc(team);
        InitTowerUIRpc(team);

        _attackDetectRange = GetAttackRange();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InitTowerUIRpc(UnitTeamType team)
    {
        UIManager.Instance.TowerInit(_hpController, team);
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (_target == null)
        {
            FindUnitInRangeRpc();

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
                return;
            }
            else
            {
                _attackCoroutine = StartCoroutine(AttackCoroutine(_target, 1.0f, 1.0f));
            }
        }
    }
    public override void Dead()
    {
        if (!IsOwner)
        {
            return;
        }

        StopAttack();
        GameManager.Instance.GameOverRpc(_teamType.Value);
        ParticleManager.Instance.ParticlePlay(ParticleType.TowerDestruction, transform.position);
        NetworkObject.Despawn();
    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHPRpc(-damage);
    }
}
