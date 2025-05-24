using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScreenHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _redTeamHPBar;
    [SerializeField] private Slider _blueTeamHPBar;
    [SerializeField] private TMP_Text _redTeamHPText;
    [SerializeField] private TMP_Text _blueTeamHPText;

    public void UpdateRedTeamHPBar(float maxHP, float currentHP)
    {
        UpdateUI(_redTeamHPBar, maxHP, currentHP);

        UpdateHPText(_redTeamHPText, maxHP, currentHP);
    }

    public void UpdateBlueTeamHPBar(float maxHP, float currentHP)
    {
        UpdateUI(_blueTeamHPBar, maxHP, currentHP);

        UpdateHPText(_blueTeamHPText, maxHP, currentHP);
    }

    private void UpdateUI(Slider targetBar, float maxHP, float currentHP)
    {
        if (currentHP >= 0 && maxHP > 0)
        { 
            targetBar.value = currentHP / maxHP;
        }
    }

    private void UpdateHPText(TMP_Text targetText, float maxHP, float currentHP)
    {
        if (currentHP >= 0 && maxHP > 0)
        {
            targetText.text = $"{currentHP}/{maxHP}";
        }
        else
        {
            targetText.text = "0/0";
        }
    }
}

