using NUnit.Framework;
using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : UnitController
{
    [SerializeField] private Animator _modelAnimator;
    [SerializeField] private LayerMask _groundMask;

    /// <summary>
    /// 레벨에 따른 부활시간 RESAPWN_TIME[현재레벨], 단위는 초
    /// </summary>
    const float RESPAWN_TIME = 8.0f;
    const float MOVE_STOPPING_DISTANCE = 3.0f;

    public bool IsAttackButtonDown { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    private new void Start()
    {
        base.Start();

        if (!IsOwner)
        {
            return;
        }

        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftMouseDown);
        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightMouseDown);
        PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(OnAttackButtonDown);

        PlayerInputManager.Instance.OnSkillButtonEvent.AddListener(OnSkillButtonDown);
    }

    public void Init(UnitTeamType team, ulong clientId)
    {
        NetworkObject.SpawnAsPlayerObject(clientId);
        SetTeamTypeRpc(team);
        UIInitRpc(team);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UIInitRpc(UnitTeamType team)
    {
        //UIManager.Instance.Init(_hpController, team, _unitStatusController.GetHeroPortrait());
        HeroHpBarUI heroHpBarUI = _unitHPBarUI as HeroHpBarUI;

        heroHpBarUI.UpdateName(GetHeroName());
        //heroHpBarUI.UpdateLevel(GetLevel());
        heroHpBarUI.SetHeroPortrait(_unitStatusController.GetHeroPortrait());

        if (IsOwner)
        {
            FindAnyObjectByType<CinemachineCamera>().Follow = transform;
        }

        UIManager.Instance.SetHeroPortrait(_unitStatusController.GetHeroPortrait());
        UIManager.Instance.InitializePlayerStatus(_unitStatusController);
    }

    private void OnAttackButtonDown()
    {
        IsAttackButtonDown = true;
    }

    public override void Dead()
    {
        IsDead.Value = true;
        StopMove();
        _target = null;
        _collider.enabled = false;

        StartCoroutine(WaitRespawnCoroutine(RESPAWN_TIME));
        UIManager.Instance.EnableRespawnPanel(RESPAWN_TIME);
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
            UIManager.Instance.UpdateRespawnTimer(waitTime - elapsedTime);

            Logger.Info($"Logger.Info: {logCounter}초 경과");
            logCounter++;
        }
        Respawn();
    }

    public void Respawn()
    {
        Logger.Info("Respawn");
        UIManager.Instance.DisableRespawnPanel();
        IsDead.Value = false;
        _collider.enabled = true;
        transform.position = GameManager.Instance.GetRespawnPoint(TeamType);
        _hpController.Init(_unitStatusController.GetMaxHP());
    }

    public override void ReceiveDamage(float damage)
    {
        if (!IsDead.Value)
        { 
            _hpController.ChangeHPRpc(-damage);
        }
    }

    public void ReceiveHeal(float heal)
    {
        _hpController.ChangeHPRpc(heal);
    }

    private void Update()
    {
        if (IsHost)
        {
            SetAnimatorBoolRpc("IsRun", _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance);
        }

        if (!IsOwner)
        {
            return;
        }

        if (_target == null)
        {
            return;
        }

        if (_target.IsDead.Value)
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
                StopMove();
            }
        }
        else
        {
            float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
            if (distanceToTarget > MOVE_STOPPING_DISTANCE)
            {
                SetMoveDestinationRpc(_target.transform.position);
            }
            else
            {
                StopMove();
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SetAnimatorBoolRpc(string name, bool param)
    {
        _modelAnimator.SetBool(name, param);
    }

    public void OnLeftMouseDown()
    {
        if (!IsOwner)
        {
            return;
        }

        IsAttackButtonDown = false;

        if (IsDead.Value)
        {
            Logger.Info("IsDead 켜져있음");
            return;
        }

        if (IsAttackButtonDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit unitHit, 100f, _unitLayerMask))
            {
                GameObject hitObject = unitHit.collider.gameObject;

                if (hitObject.TryGetComponent<UnitController>(out var unit))
                {
                    if (unit.TeamType != TeamType)
                    {
                        _target = unit;
                    }
                }
            }
            else if (Physics.Raycast(ray, out RaycastHit hit, 100f, _groundMask))
            {
                SetMoveDestinationRpc(hit.point);
            }
        }
    }

    public void OnRightMouseDown()
    {
        if (!IsOwner)
        {
            return;
        }

        IsAttackButtonDown = false;

        if (IsDead.Value)
        {
            Logger.Info("Is Dead");
            return;
        }

        _target = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit unitHit, 100f, _unitLayerMask))
        {
            GameObject hitObject = unitHit.collider.gameObject;

            if (hitObject.TryGetComponent<UnitController>(out var unit))
            {
                Logger.Info($"Mouse Hit Unit: {unit.name}");
                if (unit.TeamType != TeamType)
                { 
                    _target = unit;
                }
            }
        }
        else if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, _groundMask))
        {
            SetMoveDestinationRpc(groundHit.point);
        }
    }

    [Rpc(SendTo.Server)]
    public void BlinkToRpc(Vector3 position)
    {
        _navMeshAgent.Warp(position);
    }

    public void OnSkillButtonDown(ButtonType buttonType)
    {
        if (!IsOwner)
        {
            return;
        }

        ItemData useItem = _unitStatusController.RemoveAndGetItem(buttonType);
        useItem?.Use(this);
        UIManager.Instance.SetItemImage(buttonType, null);
        Logger.Info($"아이템 슬롯 {buttonType} 사용");
    }

    public void AddItem(ItemData item)
    {
        if (!_unitStatusController.IsItemListFull())
        {
            if (_unitStatusController.IsItemSlotEmpty(ButtonType.Q))
            { 
                _unitStatusController.AddItem(item, ButtonType.Q);
                UIManager.Instance.SetItemImage(ButtonType.Q, item.ItemSO.ItemSprite);
            }
            else if (_unitStatusController.IsItemSlotEmpty(ButtonType.W))
            {
                _unitStatusController.AddItem(item, ButtonType.W);
                UIManager.Instance.SetItemImage(ButtonType.W, item.ItemSO.ItemSprite);

            }
            else if (_unitStatusController.IsItemSlotEmpty(ButtonType.E))
            {
                _unitStatusController.AddItem(item, ButtonType.E);
                UIManager.Instance.SetItemImage(ButtonType.E, item.ItemSO.ItemSprite);
            }
        }
    }
}
