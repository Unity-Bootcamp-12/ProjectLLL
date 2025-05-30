using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class HPController : NetworkBehaviour
{
    [SerializeReference] private NetworkVariable<float> _currentHP;
    [SerializeReference] private NetworkVariable<float> _maxHP;

    private UnitController _unitController;

    public UnityEvent<float, float> OnChangeHPEvent = new();
    public UnityEvent OnDeadEvent = new UnityEvent();

    private void Awake()
    {
        _unitController = GetComponent<UnitController>();
        _currentHP = new NetworkVariable<float>(0);
        _maxHP = new NetworkVariable<float>(0);
    }

    public void Init(float maxHP)
    {
        _currentHP.OnValueChanged += (oldValue, newValue) =>
        {
            HPChangeEventRpc();
        };
        _maxHP.OnValueChanged += (oldValue, newValue) =>
        {
            HPChangeEventRpc();
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
        HPChangeEventRpc();
    }

    [Rpc(SendTo.Server)]
    public void SetMaxHPRpc(float hp)
    {
        _maxHP.Value = hp;
        if (_currentHP.Value > hp)
        { 
            _currentHP.Value = hp;
        }
        HPChangeEventRpc();
    }

    [Rpc(SendTo.Server)]
    public void ChangeHPRpc(float damage)
    {
        _currentHP.Value = Mathf.Clamp(_currentHP.Value + damage, 0, _maxHP.Value);
        HPChangeEventRpc();

        if (_currentHP.Value <= 0)
        {
            DeadEventRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void HPChangeEventRpc()
    {
        OnChangeHPEvent?.Invoke(_maxHP.Value, _currentHP.Value);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DeadEventRpc()
    {
        OnDeadEvent?.Invoke();
    }
}
