using UnityEngine;
using UnityEngine.UI;

public class PlayerScreenHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _redTeamHPBar;
    [SerializeField] private Slider _blueTeamHPBar;

    public void UpdateRedTeamHPBar(float maxHP, float currentHP)
    {
        UpdateUI(_redTeamHPBar, maxHP, currentHP);
    }

    public void UpdateBlueTeamHPBar(float maxHP, float currentHP)
    {
        UpdateUI(_blueTeamHPBar, maxHP, currentHP);
    }

    private void UpdateUI(Slider targetBar, float maxHP, float currentHP)
    {
        if (currentHP >= 0 && maxHP > 0)
        { 
            targetBar.value = currentHP / maxHP;
        }
    }
}

