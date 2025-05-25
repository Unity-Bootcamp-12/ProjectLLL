using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScreenHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _redTeamHPBar;
    [SerializeField] private Slider _blueTeamHPBar;
    [SerializeField] private TMP_Text _redTeamHPText;
    [SerializeField] private TMP_Text _blueTeamHPText;
    [SerializeField] private GameObject _redTeamPortrait;
    [SerializeField] private GameObject _blueTeamPortrait;

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

    public void SetHeroPortrait(UnitTeamType teamType, Sprite heroPortrait)
    {
        if (heroPortrait != null)
        {
            if (teamType == UnitTeamType.RedTeam)
            {
                if (_redTeamPortrait.TryGetComponent(out Image img))
                {
                    img.sprite = heroPortrait;
                }
            }
            else if (teamType == UnitTeamType.BlueTeam)
            {
                if (_blueTeamPortrait.TryGetComponent(out Image img))
                {
                    img.sprite = heroPortrait;
                }
            }
        }
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

