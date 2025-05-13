using UnityEngine;

/// <summary>
/// 챔피언, 미니언, 타워 등 HP를 가진 유닛의 Base 클래스
/// </summary>
public abstract class UnitController : MonoBehaviour
{
    public HPController HPController { get; private set; }
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

    private void Awake()
    {
        HPController = GetComponent<HPController>();
    }
}

public enum UnitTeamType
{
    RedTeam,
    BlueTeam,
}