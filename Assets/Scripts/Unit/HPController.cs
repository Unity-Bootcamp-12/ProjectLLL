using UnityEngine;
using UnityEngine.Events;

public class HPController : MonoBehaviour
{
    [SerializeReference] private float _currentHP;
    [SerializeReference] private float _maxHP;

    public UnityEvent OnChangeHPEvent = new UnityEvent();
    public UnityEvent OnDeadEvent = new UnityEvent();

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
        OnChangeHPEvent?.Invoke();

        if (_currentHP <= 0)
        {
            OnDeadEvent?.Invoke();
        }
    }
}
