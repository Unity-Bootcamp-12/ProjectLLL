using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerScreenHUDUI : MonoBehaviour
{
    [SerializeField] private GameObject _heroPortrait;
    [SerializeField] private TextMeshPro _levelText;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _xpBar;
    [SerializeField] private Button _qSkillButton;
    [SerializeField] private Button _wSkillButton;
    [SerializeField] private Button _eSkillButton;
    [SerializeField] private Image _qCooldownOverlay;
    [SerializeField] private Image _wCooldownOverlay;
    [SerializeField] private Image _eCooldownOverlay;

    [SerializeField] private GameObject _respawnWaitPanel;
    [SerializeField] private TextMeshPro _respawnTimerText;

    public UnityEvent OnQSkillEvent = new();
    public UnityEvent OnWSkillEvent = new();
    public UnityEvent OnESkillEvent = new();

    private void Awake()
    {
        _qSkillButton.onClick.AddListener(() => OnQSkillEvent.Invoke());
        _wSkillButton.onClick.AddListener(() => OnWSkillEvent.Invoke());
        _eSkillButton.onClick.AddListener(() => OnESkillEvent.Invoke());
    }

    public void SetHeroPortraitImage(Sprite sprite)
    {
        if (_heroPortrait.TryGetComponent(out Image img))
            img.sprite = sprite;
    }

    public void UpdateHpBar(float maxHP, float currentHP)
    {
        _hpBar.value = Mathf.Clamp01(currentHP / Mathf.Max(1, maxHP));
    }

    public void UpdateXPBar(float maxXP, float currentXP)
    {
        _xpBar.value = Mathf.Clamp01(currentXP / Mathf.Max(1, maxXP));
    }

    public void SetSkillCooldown(SkillType type, float cooldown, float maxCooldown)
    {
        float fill = Mathf.Clamp01(cooldown / Mathf.Max(1, maxCooldown));
        switch (type)
        {
            case SkillType.Q: _qCooldownOverlay.fillAmount = fill; break;
            case SkillType.W: _wCooldownOverlay.fillAmount = fill; break;
            case SkillType.E: _eCooldownOverlay.fillAmount = fill; break;
        }
    }
    public enum SkillType { Q, W, E }

    public void SetLevel(int level)
    {
        _levelText.text = level.ToString();
    }

    public void ShowRespawnWaitPanel(int respawnTime)
    {
        Canvas rootCanvas = _respawnWaitPanel.GetComponentInParent<Canvas>().rootCanvas;
        RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();
        RectTransform panelRect = _respawnWaitPanel.GetComponent<RectTransform>();

        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelRect.sizeDelta = canvasRect.sizeDelta;
        panelRect.anchoredPosition = Vector2.zero;

        _respawnWaitPanel.SetActive(true);
        _respawnTimerText.text = respawnTime.ToString();
    }

    public void UpdateRespawnTimer(int remainingSeconds)
    {
        _respawnTimerText.text = remainingSeconds.ToString();
    }

    public void HideRespawnPanel()
    {
        _respawnWaitPanel.SetActive(false);
    }

}
