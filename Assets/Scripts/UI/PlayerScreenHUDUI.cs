using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType { Q, W, E }

public class PlayerScreenHUDUI : MonoBehaviour
{
    [SerializeField] private GameObject _heroPortrait;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _xpBar;
    [SerializeField] private Button _qSkillButton;
    [SerializeField] private Button _wSkillButton;
    [SerializeField] private Button _eSkillButton;
    [SerializeField] private TMP_Text _HPText;

    private void Awake()
    {
        _qSkillButton.onClick.AddListener(() => PlayerInputManager.Instance.OnSkillButtonEvent?.Invoke(ButtonType.Q));
        _wSkillButton.onClick.AddListener(() => PlayerInputManager.Instance.OnSkillButtonEvent?.Invoke(ButtonType.W));
        _eSkillButton.onClick.AddListener(() => PlayerInputManager.Instance.OnSkillButtonEvent?.Invoke(ButtonType.E));
    }

    public void SetHeroPortraitImage(Sprite sprite)
    {
        if (_heroPortrait.TryGetComponent(out Image img))
        {
            img.sprite = sprite;
        }
    }

    public void SetButtonImage(ButtonType button, Sprite sprite)
    {
        switch (button)
        {
            case ButtonType.Q:
                _qSkillButton.image.sprite = sprite;
                break;
            case ButtonType.W:
                _wSkillButton.image.sprite = sprite;
                break;
            case ButtonType.E:
                _eSkillButton.image.sprite = sprite;
                break;
        }
    }

    public void UpdateHpBar(float maxHP, float currentHP)
    {
        _hpBar.value = Mathf.Clamp01(currentHP / Mathf.Max(1, maxHP));
        _HPText.text = $"{currentHP}/{maxHP}";
    }

    public void UpdateXPBar(float maxXP, float currentXP)
    {
        _xpBar.value = Mathf.Clamp01(currentXP / Mathf.Max(1, maxXP));
    }

    public void SetLevel(int level)
    {
        _levelText.text = level.ToString();
    }
}
