using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : UnitController
{
    [SerializeField] private Animator _modelAnimator;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private Transform _respawnAnchor;//나중에 할당으로 수정해야 함

    #region 5/22 수정한 부분(경민)
    [Header("Get Item")]
    [SerializeField] private int _maxItemSlots = 3;
    private ItemScriptableObject[] _itemSlots;
    private int _nextSlot = 0;
    #endregion

    /// <summary>
    /// 레벨에 따른 부활시간 RESAPWN_TIME[현재레벨], 단위는 초
    /// </summary>
    readonly float[] RESPAWN_TIME = { 0.0f, 5.0f, 10.0f, 15.0f, 20.0f, 25.0f, 30.0f, 35.0f, 40.0f, 45.0f };

    public bool IsAttackButtonDown { get; private set; }

    const float MOVE_STOPPING_DISTANCE = 3.0f;

    protected override void Awake()
    {
        base.Awake();
        #region 5/22 수정한 부분(경민)
        _itemSlots = new ItemScriptableObject[_maxItemSlots];
        #endregion
    }

    private new void Start()
    {
        base.Start();

        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftMouseDown);
        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightMouseDown);
        PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(OnAttackButtonDown);

        #region 5/22 수정한 부분(경민)
        PlayerInputManager.Instance.OnQSkillEvent.AddListener(() => UseItem(0));
        PlayerInputManager.Instance.OnWSkillEvent.AddListener(() => UseItem(1));
        PlayerInputManager.Instance.OnESkillEvent.AddListener(() => UseItem(2));
        #endregion
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
        UIManager.Instance.Init(_hpController, team);
        HeroHpBarUI heroHpBarUI = _unitHPBarUI as HeroHpBarUI;

        heroHpBarUI.UpdateName(GetHeroName());
        heroHpBarUI.UpdateLevel(GetLevel());

        if (IsOwner)
        {
            FindAnyObjectByType<CinemachineCamera>().Follow = transform;
        }

        UIManager.Instance.SetHeroPortrait(_unitStatusController.GetHeroPortrait());
    }

    private void OnLeftMouseDown1()
    {
        IsAttackButtonDown = false;
    }

    private void OnRightMouseDown1()
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
        StopMove();
        _collider.enabled = false;

        UIManager.Instance.EnableRespawnPanel(RESPAWN_TIME[_unitStatusController.GetLevel()]);

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
        IsDead = false;
        _collider.enabled = true;
        transform.position = _respawnAnchor.position;
        _hpController.Init(_unitStatusController.GetMaxHP());
    }

    public override void ReceiveDamage(float damage)
    {
        _hpController.ChangeHPRpc(-damage);
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

        if (_target.IsDead)
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

        if (IsDead)
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

        if (IsDead)
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
                _target = unit;
            }
        }
        else if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, _groundMask))
        {
            SetMoveDestinationRpc(groundHit.point);
        }
    }

    #region 5/22 수정한 부분(경민)
    private void OnTriggerEnter(Collider other)
    {
        var consumable = other.GetComponent<ItemConsumable>();
        if (consumable == null) return;

        if(_nextSlot < _maxItemSlots)
        {
            _itemSlots[_nextSlot] = consumable.Item;
            Logger.Info($"[스킬창] Slot {_nextSlot + 1}에 {consumable.Item.name} 수집함.");
            _nextSlot++;
            Destroy(other.gameObject);
        }
        else
        {
            Logger.Warning("스킬창 슬롯이 가득 참.");
        }
    }

    private void UseItem(int slotIndex)
    {
        if(slotIndex < 0
            || slotIndex >= _nextSlot
            || _itemSlots[slotIndex] == null) return;

        var item = _itemSlots[slotIndex];
        item.Use(this);

        Logger.Info($"[스킬창] Slot {slotIndex + 1}의 {item.name} 사용함.");
    }

    private void OnDisable()
    {
        if(PlayerInputManager.Instance == null) return;
        PlayerInputManager.Instance.OnQSkillEvent.RemoveListener(() => UseItem(0));
        PlayerInputManager.Instance.OnWSkillEvent.RemoveListener(() => UseItem(1));
        PlayerInputManager.Instance.OnESkillEvent.RemoveListener(() => UseItem(2));
    }
    #endregion
}