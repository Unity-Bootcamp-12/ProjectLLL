using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : UnitController
{
    [SerializeField] private Transform _respawnAnchor;//나중에 할당으로 수정해야 함
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _heroNameText;

    private PlayerAction _playerAction;
    /// <summary>
    /// 레벨에 따른 부활시간 RESAPWN_TIME[현재레벨], 단위는 초
    /// </summary>
    readonly float[] RESPAWN_TIME = { 0.0f, 5.0f, 10.0f, 15.0f, 20.0f, 25.0f, 30.0f, 35.0f, 40.0f, 45.0f };
    
    public bool IsAttackButtonDown { get; private set; }

    private new void Awake()
    {
        base.Awake();
        _playerAction = GetComponent<PlayerAction>();
    }

    private new void Start()
    {
        base.Start();

        Logger.Info($"챔피언 이름 : {GetHeroName()}");
        _heroNameText.text = GetHeroName();

        Logger.Info($"현재 레벨 : {GetLevel()}");
        _levelText.text = GetLevel().ToString();

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
        }, RESPAWN_TIME[_unitStatusController.GetLevel()]);
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