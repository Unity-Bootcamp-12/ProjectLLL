using UnityEngine;

/// <summary>
/// 챔피언, 미니언, 타워 등 HP를 가진 유닛의 Base 클래스
/// </summary>
public abstract class UnitController : MonoBehaviour
{
    [SerializeField] private UnitHpBarUI _unitHpBarUI;

    protected HPController _hpController;
    protected UnitStatusController _unitStatusController;
    protected Collider _collider;

    public UnitTeamType TeamType { get; private set; }

    public bool IsDead { get; protected set; }

    public UnitTeamType GetTeamType()
    {
        return TeamType;
    }

    public void SetTeamType(UnitTeamType teamType)
    {
        TeamType = teamType;
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
        _unitHpBarUI.Init(_hpController.OnChangeHPEvent);
    }
}

public enum UnitTeamType
{
    RedTeam,
    BlueTeam,
}