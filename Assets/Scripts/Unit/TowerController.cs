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
                SoundManager.Instance.PlaySFX(SFXType.NexusCannon);
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
        SoundManager.Instance.PlaySFX(SFXType.NexusExplode);
        GameManager.Instance.GameOverRpc(_teamType.Value);
        NetworkObject.Despawn();
    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHPRpc(-damage);
    }
}
