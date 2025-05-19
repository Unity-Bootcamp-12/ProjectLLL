using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class HPController : NetworkBehaviour
{
    [SerializeReference] private NetworkVariable<float> _currentHP;
    [SerializeReference] private NetworkVariable<float> _maxHP;

    public UnityEvent<float, float> OnChangeHPEvent = new UnityEvent<float, float>();
    public UnityEvent OnDeadEvent = new UnityEvent();

    private void Awake()
    {
        _currentHP = new NetworkVariable<float>(0);
        _maxHP = new NetworkVariable<float>(0);
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

    [Rpc(SendTo.Server)]
    public void SetCurrentHPRpc(float hp)
    {
        _currentHP.Value = hp;
        HPChangeRpc();
    }

    public float GetMaxHP()
    {
        return _maxHP.Value;
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
}