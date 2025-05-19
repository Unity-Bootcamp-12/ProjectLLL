using UnityEngine;
using UnityEngine.UI;

public class PlayerScreenHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _redTeamHPBar;
    [SerializeField] private Slider _blueTeamHPBar;

    public void UpdateTeamHP(UnitTeamType teamType, float maxHP, float currentHP)
    {
        Slider targetBar = teamType == UnitTeamType.RedTeam ? _redTeamHPBar : _blueTeamHPBar;

        if (targetBar != null)
        {
            targetBar.value = currentHP / maxHP;
        }
    }
}

