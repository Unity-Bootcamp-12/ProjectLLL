using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroHpBarUI : UnitHPBarUI
{
    //[SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _heroNameText;
    [SerializeField] private GameObject _heroPortrait;

    public void UpdateName(string name)
    {
        _heroNameText.text = name;
    }

    //public void UpdateLevel(int level)
    //{
    //    if (_levelText != null && _levelText.IsActive())
    //    {
    //        _levelText.text = level.ToString();
    //    }
    //}

    public void SetHeroPortrait(Sprite heroPortrait)
    {
        if (heroPortrait != null && _heroPortrait.TryGetComponent(out Image img))
        {
            img.sprite = heroPortrait;
        }
    }
}
