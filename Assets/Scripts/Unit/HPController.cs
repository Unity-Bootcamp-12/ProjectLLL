using UnityEngine;
using UnityEngine.Events;

public class HPController : MonoBehaviour
{
    private float _currentHP;
    private float _maxHP;

    public UnityEvent OnChangeHP = new UnityEvent();
    public UnityEvent OnDead = new UnityEvent();

    public void Init(float maxHP)
    {
        _maxHP = maxHP;
        _currentHP = maxHP;
    }

    public float GetCurrentHP()
    {
        return _currentHP;
    }

    public void SetCurrentHP(float hp)
    {
        _currentHP = hp;
    }

    public float GetMaxHP()
    {
        return _maxHP;
    }

    public void SetMaxHP(float hp)
    {
        _maxHP = hp;
    }

    public void ChangeHP(float damage)
    {
        _currentHP = Mathf.Clamp(_currentHP + damage, 0, _maxHP);
        OnChangeHP?.Invoke();

        if (_currentHP <= 0)
        {
            OnDead?.Invoke();
        }
    }
}
