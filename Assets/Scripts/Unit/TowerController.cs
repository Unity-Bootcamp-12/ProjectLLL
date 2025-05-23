using UnityEngine;

public class TowerController : UnitController
{
    public override void Dead()
    {
        StopAttack();
        GameManager.Instance.GameOverRpc(_teamType.Value);
        NetworkObject.Despawn();
    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHPRpc(-damage);
    }

    public void Init(UnitTeamType team)
    {
        NetworkObject.Spawn();
        SetTeamTypeRpc(team);
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (_target == null)
        {
            FindUnitInRange();

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
            }
        }
    }
}
