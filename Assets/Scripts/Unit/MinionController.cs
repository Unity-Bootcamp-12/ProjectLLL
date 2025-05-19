using UnityEngine;

public class MinionController : UnitController
{
    public override void Dead()
    {

    }

    public override void OnTriggerEnterFromChild(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTriggerExitFromChild(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHPRpc(-damage);
    }
}
