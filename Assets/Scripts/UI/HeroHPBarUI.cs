using TMPro;
using UnityEngine;

public class HeroHpBarUI : UnitHPBarUI
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _heroNameText;

    public void UpdateName(string name)
    {
        _heroNameText.text = name;
    }

    public void UpdateLevel(int level)
    {
        _levelText.text = level.ToString();
    }
}
