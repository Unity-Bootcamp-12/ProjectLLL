using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : UnitController
{
    [SerializeField] private Transform _respawnAnchor;//나중에 할당으로 수정해야 함

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

        HeroHpBarUI heroHpBarUI = _unitHPBarUI as HeroHpBarUI;
        
        Logger.Info($"영웅 이름 : {GetHeroName()}");
        Logger.Info($"현재 레벨 : {GetLevel()}");

        heroHpBarUI.UpdateName(GetHeroName());
        heroHpBarUI.UpdateLevel(GetLevel());

        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftMouseDown);
        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightMouseDown);
        PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(OnAttackButtonDown);

        // TEST
        if (IsOwner)
        {
            FindAnyObjectByType<CinemachineCamera>().Follow = transform;
        }

        if (IsLocalPlayer)
        {
            gameObject.name = "PLAYER";
            if (IsHost)
            {
                SetTeamTypeRpc(UnitTeamType.RedTeam);
                heroHpBarUI.UpdateName("RED");
            }
            else
            {
                SetTeamTypeRpc(UnitTeamType.BlueTeam);
                heroHpBarUI.UpdateName("BLUE");
            }
        }
        else
        {
            gameObject.name = "OTHER";
        }
    }

    private void OnLeftMouseDown()
    {
        IsAttackButtonDown = false;
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

        StartCoroutine(WaitRespawnCoroutine(RESPAWN_TIME[_unitStatusController.GetLevel()]));
    }

    /// <summary>
    /// 리스폰 대기시간 (임시)
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private IEnumerator WaitRespawnCoroutine(float waitTime)
    {
        float elapsedTime = 0.0f;
        int logCounter = 1;

        while (elapsedTime < waitTime)
        {
            yield return new WaitForSeconds(1.0f);
            elapsedTime += 1f;
            Logger.Info($"Logger.Info: {logCounter}초 경과");
            logCounter++;
        }
        Respawn();
    }

    public void Respawn()
    {
        Logger.Info("Respawn");
        IsDead = false;
        _collider.enabled = true;
        transform.position = _respawnAnchor.position;
        _hpController.Init(_unitStatusController.GetMaxHP());
    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHPRpc(-damage);
    }

    public override void OnTriggerEnterFromChild(Collider other)
    {
        throw new NotImplementedException();
    }

    public override void OnTriggerExitFromChild(Collider other)
    {
        throw new NotImplementedException();
    }
}
