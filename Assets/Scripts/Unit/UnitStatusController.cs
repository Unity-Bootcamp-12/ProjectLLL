using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

    [SerializeField] private int _experience = 0;
    [SerializeField] private int _level = 1;
    [SerializeField] private int _usedSkillPoints = 0;
    const int MAX_LEVEL = 9;

    private void Start()
    {
        _changeStatus = new UnitStatus();
    }

    #region 스탯 관련
    public float GetMaxHP() => _heroSO.HeroStatus.MaxHP + _changeStatus.MaxHP;
    public float GetAttackPower() => _heroSO.HeroStatus.AttackPower + _changeStatus.AttackPower;
    public float GetAttackSpeed() => _heroSO.HeroStatus.AttackSpeed + _changeStatus.AttackSpeed;
    public float GetAttackRange() => _heroSO.HeroStatus.AttackRange + _changeStatus.AttackRange;
    public float GetMoveSpeed() => _heroSO.HeroStatus.MoveSpeed + _changeStatus.MoveSpeed; 
    #endregion

    #region 경험치 관련
    public int GetExperience() => _experience;
    private int SetExperience(int value)
    {
        _experience = Mathf.Clamp(value, 0, _heroSO.GetAmountOfExperience()[_level]);

        if (_experience >= _heroSO.GetAmountOfExperience()[_level])
        {
            AddLevel();
        }

        return _experience;
    }

    /// <summary>
    /// 경험치 증가
    /// </summary>
    /// <param name="addExp"></param>
    /// <returns></returns>
    private int AddExperience(int addExp)
    {
        int remainingExp = addExp;

        while (remainingExp > 0 && _level < MAX_LEVEL)
        {
            int requiredExp = _heroSO.GetAmountOfExperience()[_level] - _experience;
            int gainedExp = Mathf.Min(remainingExp, requiredExp);

            _experience += gainedExp;
            remainingExp -= gainedExp;

            if (_experience >= _heroSO.GetAmountOfExperience()[_level])
            {
                AddLevel();
            }
        }
        return _experience;
    }
    #endregion

    #region 레벨 관련
    public int GetLevel() => _level;
    private int SetLevel(int value)
    {
        _level = Mathf.Clamp(value, 0, MAX_LEVEL);
        return _level;
    }
    private int AddLevel()
    {
        if (_level >= MAX_LEVEL)
        {
            return _level;
        }

        _level = Mathf.Clamp(++_level, 0, MAX_LEVEL);
        return _level;
    }
    #endregion

    #region 스킬포인트 관련
    public int AvailableSkillPoints => _level - _usedSkillPoints;
    public int GetUsedSkillPoints() => _usedSkillPoints;

    /// <summary>
    /// AvailableSkillPoints가 있나 조건 확인 후 AddUsedSkillPoints로 사용한 스킬 포인트 증가 = 스킬 포인트 사용
    /// </summary>
    /// <returns></returns>
    public bool UseSkillPoint()
    {
        if (AvailableSkillPoints <= 0)
        {
            // 사용할 수 있는 스킬포인트가 없으면 return
            return false;
        }
        _usedSkillPoints = Mathf.Min(_level, ++_usedSkillPoints);
        return true;
    }
    #endregion
}
