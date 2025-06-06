using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScreenStatusUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _attackPowerCurrentValue; 
    [SerializeField] private TMP_Text _attackPowerIncreaseValue; 
    [SerializeField] private TMP_Text _attackSpeedCurrentValue; 
    [SerializeField] private TMP_Text _attackSpeedIncreaseValue; 
    [SerializeField] private TMP_Text _attackRangeCurrentValue; 
    [SerializeField] private TMP_Text _attackRangeIncreaseValue; 
    [SerializeField] private TMP_Text _MoveSpeedCurrentValue; 
    [SerializeField] private TMP_Text _MoveSpeedIncreaseValue;

    public void UpdateUI(UnitStatusController playerStatus)
    {
        _attackPowerCurrentValue.text = $"{playerStatus.GetAttackPower()}";
        _attackSpeedCurrentValue.text = $"{playerStatus.GetAttackSpeed()}";
        _attackRangeCurrentValue.text = $"{playerStatus.GetAttackRange()}";
        _MoveSpeedCurrentValue.text = $"{playerStatus.GetMoveSpeed()}";

        _attackPowerIncreaseValue.text = $"(+{playerStatus.ChangeStatus.AttackPower})";
        _attackSpeedIncreaseValue.text = $"(+{playerStatus.ChangeStatus.AttackSpeed})";
        _attackRangeIncreaseValue.text = $"(+{playerStatus.ChangeStatus.AttackRange})";
        _MoveSpeedIncreaseValue.text = $"(+{playerStatus.ChangeStatus.MoveSpeed})";
    }
}
