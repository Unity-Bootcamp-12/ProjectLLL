using System;
using UnityEngine;

public class PlayerController : UnitController
{
    private PlayerMovement _playerMovement;
    public bool IsAttackButtonDown { get; private set; }

    private void Start()
    {
        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftMouseDown);
        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightMouseDown);
        PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(OnAttackButtonDown);
    }

    private void OnLeftMouseDown()
    {
    }

    private void OnRightMouseDown()
    {
        IsAttackButtonDown = false;
    }

    private void OnAttackButtonDown()
    {
        IsAttackButtonDown = true;
    }

    public override void Dead()
    {
        IsDead = true;
    }

    public override void ReceiveDamage(float damage)
    {
        HPController.ChangeHP(-damage);
    }

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }
}