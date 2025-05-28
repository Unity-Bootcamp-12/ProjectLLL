using NUnit.Framework;
using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : UnitController
{
    [SerializeField] private LayerMask _groundMask;

    const float RESPAWN_TIME = 8.0f;

    private bool _isAttackButtonDown;
    private bool _isAttackSearching;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (!IsOwner)
        {
            return;
        }

        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftMouseDown);
        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightMouseDown);
        PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(OnAttackButtonDown);

        PlayerInputManager.Instance.OnSkillButtonEvent.AddListener(OnSkillButtonDown);
    }

    public override void Init(UnitTeamType team, ulong clientId)
    {
        NetworkObject.SpawnAsPlayerObject(clientId);

        base.Init(team, clientId);

        SetTeamTypeRpc(team);
        InitPlayerUIRpc(team);

        _attackDetectRange = Mathf.Clamp(GetAttackRange() * 2.0f, 6.0f, 10.0f);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InitPlayerUIRpc(UnitTeamType team)
    {
        HeroHpBarUI heroHpBarUI = _unitHPBarUI as HeroHpBarUI;

        heroHpBarUI.UpdateName(GetHeroName());
        heroHpBarUI.SetHeroPortrait(_unitStatusController.GetHeroPortrait());

        if (IsOwner)
        {
            UIManager.Instance.Init(_hpController, team, _unitStatusController.GetHeroPortrait());
            UIManager.Instance.SetHUDHeroPortrait(_unitStatusController.GetHeroPortrait());
            UIManager.Instance.UpdatePlayerStatus(_unitStatusController);

            FindAnyObjectByType<CinemachineCamera>().Follow = transform;
        }
    }

    private void OnAttackButtonDown()
    {
        _isAttackButtonDown = true;
    }

    public override void Dead()
    {
        if (!IsOwner)
        {
            return;
        }

        Logger.Info("Player Dead");

        _target = null;
        _collider.enabled = false;
        IsDead.Value = true;
        StopMoveRpc();
        SetAnimatorTriggerRpc("IsDead");

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
        SetAnimatorTriggerRpc("IsRespawn");
        BlinkToRpc(GameManager.Instance.GetRespawnPoint(TeamType));
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
            if (_isAttackSearching)
            {
                FindUnitInRangeRpc();
            }
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
                LookAtRpc(_target.transform.position);

                return;
            }
            else
            {
                _attackCoroutine = StartCoroutine(AttackCoroutine(_target, 1.0f, 1.0f));
                StopMoveRpc();
            }
        }
        else
        {
            Vector3 selfPos = transform.position;
            Vector3 targetPos = _target.transform.position;

            selfPos.y = 0;
            targetPos.y = 0;

            float distanceToTarget = Vector3.Distance(selfPos, targetPos);
            if (distanceToTarget > MOVE_STOPPING_DISTANCE)
            {
                SetMoveDestinationRpc(_target.transform.position);
            }
            else
            {
                StopMoveRpc();
            }
        }
    }



    public void OnLeftMouseDown()
    {
        if (!IsOwner)
        {
            return;
        }

        if (IsDead.Value)
        {
            Logger.Info("IsDead 켜져있음");
            return;
        }

        if (_isAttackButtonDown)
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

        _isAttackButtonDown = false;
        _isAttackSearching = true;
    }

    public void OnRightMouseDown()
    {
        if (!IsOwner)
        {
            return;
        }

        _isAttackButtonDown = false;

        if (IsDead.Value)
        {
            Logger.Info("Is Dead");
            return;
        }

        _target = null;
        _isAttackSearching = false;

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
                    _isAttackSearching = true;
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
