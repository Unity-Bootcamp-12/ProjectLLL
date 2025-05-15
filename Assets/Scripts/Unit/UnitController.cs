using System;
using UnityEngine;

/// <summary>
/// 챔피언, 미니언, 타워 등 HP를 가진 유닛의 Base 클래스
/// </summary>
public abstract class UnitController : MonoBehaviour
{
    [SerializeField] private UnitHPBarUI _unitHPBarUI;

    protected Collider _collider;

    protected HPController _hpController;
    protected UnitStatusController _unitStatusController;

    public UnitTeamType TeamType => _teamType;
    [SerializeField] private UnitTeamType _teamType;

    public bool IsDead { get; protected set; }

    public void SetTeamType(UnitTeamType teamType)
    {
        _teamType = teamType;
    }

    public float GetAttackPower()
    {
        return _unitStatusController.GetAttackPower();
    }

    public float GetAttackRange()
    {
        return _unitStatusController.GetAttackRange();
    }

    public float GetAttackSpeed()
    {
        return _unitStatusController.GetAttackSpeed();
    }

    public int GetLevel()
    {
        return _unitStatusController.GetLevel();
    }

    public string GetHeroName()
    {
        return _unitStatusController.GetHeroName();
    }

    public abstract void ReceiveDamage(float damage);
    public abstract void Dead();

    protected void Awake()
    {
        Logger.Info($"UnitController Awake: {gameObject.name}");
        _hpController = GetComponent<HPController>();
        _unitStatusController = GetComponent<UnitStatusController>();
        _collider = GetComponent<Collider>();
    }

    protected void Start()
    {
        _hpController.Init(_unitStatusController.GetMaxHP());
        _hpController.OnDeadEvent.AddListener(Dead);
        _unitHPBarUI.Init(_hpController.OnChangeHPEvent);
    }
}

[Serializable]
public enum UnitTeamType
{
    RedTeam,
    BlueTeam,
}