using System;
using UnityEngine;

public class PlayerController : UnitController
{
    [SerializeField] private Transform _respawnAnchor;//나중에 할당으로 수정해야 함
    
    private PlayerAction _playerAction;
    
    public bool IsAttackButtonDown { get; private set; }

    private new void Awake()
    {
        base.Awake();
        _playerAction = GetComponent<PlayerAction>();
    }

    private new void Start()
    {
        base.Start();

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
        _playerAction.StopMove();
        _collider.enabled = false;

        GameManager.Instance.PlayAfterCoroutine(() =>
        {
            Respawn();
        }, 3.0f);
    }

    public void Respawn()
    {
        IsDead = false;
        _collider.enabled = true;
        transform.position = _respawnAnchor.position;
        _hpController.Init(_unitStatusController.GetMaxHP());
    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHP(-damage);
    }
}