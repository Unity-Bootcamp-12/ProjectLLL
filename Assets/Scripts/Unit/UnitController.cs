using UnityEngine;

/// <summary>
/// è�Ǿ�, �̴Ͼ�, Ÿ�� �� HP�� ���� ������ Base Ŭ����
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