using UnityEngine;

public class UnitStatusController : MonoBehaviour
{
    /// <summary>
    /// 변하지 않는 오리지널 스탯
    /// </summary>
    [SerializeField] private HeroScriptableObject _heroSO;
    /// <summary>
    /// 변경 스탯치
    /// </summary>
    private UnitStatus _changeStatus;
    private int _experience;
    private int _level;
    private int _skillPoints;

    public float GetMaxHP() => _heroSO.HeroStatus.MaxHP + _changeStatus.MaxHP;
    public float GetAttackPower() => _heroSO.HeroStatus.AttackPower + _changeStatus.AttackPower;
    public float GetAttackSpeed() => _heroSO.HeroStatus.AttackSpeed + _changeStatus.AttackSpeed;
    public float GetAttackRange() => _heroSO.HeroStatus.AttackRange + _changeStatus.AttackRange;
    public float GetMoveSpeed() => _heroSO.HeroStatus.MoveSpeed + _changeStatus.MoveSpeed;

    public int GetExperience() => _experience;
    public int SetExperience(int value)
    {
        _experience = value;
        // 초과하면 레벨업

        if (_experience <= 0)
        {
            _experience = 0;
        }

        return _experience;
    }
    public int GetLevel() => _level;
    public int SetLevel(int value)
    {
        _level = value;

        return _level;
    }
    public int GetSkillPoints() => _skillPoints;
    public int SetSkillPoints(int value)
    {
        _skillPoints = value;

        return _skillPoints;
    }
}
