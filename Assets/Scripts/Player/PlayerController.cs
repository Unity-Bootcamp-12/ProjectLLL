using UnityEngine;

public class PlayerController : UnitController
{
    private PlayerMovement _playerMovement;

    public override void Dead()
    {
        IsDead = true
    }

    public override void ReceiveDamage(float damage)
    {
        HPController.ChangeHP(-damage);
    }

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _playerMovement.OnRightMouseDown();
        }
    }
}
