using UnityEngine;

public class MinionController : UnitController
{
    public override void Dead()
    {

    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHPRpc(-damage);
    }
}
