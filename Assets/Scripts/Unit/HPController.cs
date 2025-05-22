using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class HPController : NetworkBehaviour
{
    [SerializeReference] private NetworkVariable<float> _currentHP;
    [SerializeReference] private NetworkVariable<float> _maxHP;

    [Header("Change Stats")]
    [SerializeReference] private NetworkVariable<float> _attackPower;
    [SerializeReference] private NetworkVariable<float> _attackSpeed;
    [SerializeReference] private NetworkVariable<float> _attackRange;
    [SerializeReference] private NetworkVariable<float> _moveSpeed;

    private UnitController _unitController;

    public UnityEvent<float, float> OnChangeHPEvent = new();
    public UnityEvent OnDeadEvent = new UnityEvent();

    public UnityEvent<float> OnChangeStatsEvent = new();

    public float AttackPower => _attackPower.Value;
    public float AttackSpeed => _attackSpeed.Value;
    public float AttackRange => _attackRange.Value;
    public float MoveSpeed => _moveSpeed.Value;

    private void Awake()
    {
        _unitController = GetComponent<UnitController>();
        _currentHP = new NetworkVariable<float>(0);
        _maxHP = new NetworkVariable<float>(0);

        _attackPower = new NetworkVariable<float>(0);
        _attackSpeed = new NetworkVariable<float>(0);
        _attackRange = new NetworkVariable<float>(0);
        _moveSpeed = new NetworkVariable<float>(0);
    }

    public void Init(float maxHP)
    {
        _currentHP.OnValueChanged += (oldValue, newValue) =>
        {
            HPChangeRpc();
        };
        _maxHP.OnValueChanged += (oldValue, newValue) =>
        {
            HPChangeRpc();
        };

        SetMaxHPRpc(maxHP);
        SetCurrentHPRpc(maxHP);
    }

    public float GetCurrentHP()
    {
        return _currentHP.Value;
    }

    public float GetMaxHP()
    {
        return _maxHP.Value;
    }

    [Rpc(SendTo.Server)]
    public void SetCurrentHPRpc(float hp)
    {
        _currentHP.Value = hp;
        HPChangeRpc();
    }

    [Rpc(SendTo.Server)]
    public void SetMaxHPRpc(float hp)
    {
        _maxHP.Value = hp;
        HPChangeRpc();
    }

    [Rpc(SendTo.Server)]
    public void ChangeHPRpc(float damage)
    {
        _currentHP.Value = Mathf.Clamp(_currentHP.Value + damage, 0, _maxHP.Value);
        HPChangeRpc();

        if (_currentHP.Value <= 0)
        {
            OnDeadEvent?.Invoke();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void HPChangeRpc()
    {
        OnChangeHPEvent?.Invoke(_maxHP.Value, _currentHP.Value);
    }

    #region 5/22 수정한 부분(경민)
    [Rpc(SendTo.Server)]
    public void SetAttackPowerRpc(float amount)
    {
        _attackPower.Value += amount;
        StatsChangeRpc(_attackPower.Value);
        Logger.Info($"[Stats] AttackPower => {_attackPower.Value} (+ {amount}");
    }

    [Rpc(SendTo.Server)]
    public void SetAttackSpeedRpc(float amount)
    {
        _attackSpeed.Value += amount;
        StatsChangeRpc(_attackSpeed.Value);
        Logger.Info($"[Stats] AttackSpeed => {_attackSpeed.Value} (+ {amount}");
    }

    [Rpc(SendTo.Server)]
    public void SetAttackRangeRpc(float amount)
    {
        _attackRange.Value += amount;
        StatsChangeRpc(_attackRange.Value);
        Logger.Info($"[Stats] AttackRange => {_attackRange.Value} (+ {amount}");
    }

    [Rpc(SendTo.Server)]
    public void SetMoveSpeedRpc(float amount)
    {
        _moveSpeed.Value += amount;
        StatsChangeRpc(_moveSpeed.Value);
        Logger.Info($"[Stats] MoveSpeed => {_moveSpeed.Value} (+ {amount}");
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StatsChangeRpc(float stats)
    {
        OnChangeStatsEvent?.Invoke(stats);
    }
    #endregion
}