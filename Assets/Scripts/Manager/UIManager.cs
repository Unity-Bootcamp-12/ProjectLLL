using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private PlayerScreenHPBarUI _playerScreenHPBarUI;
    [SerializeField] private PlayerScreenHUDUI _playerScreenHUDUI;
    [SerializeField] private GameObject _respawnWaitPanel;
    [SerializeField] private TMP_Text _respawnTimerText;

    private CursorUIController _cursorUIController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _cursorUIController = GetComponent<CursorUIController>();
    }

    private void Start()
    {
        PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(OnAttackCursor);
        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftClick);
        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightClick);
        PlayerInputManager.Instance.OnQSkillEvent.AddListener(QSkill);
        PlayerInputManager.Instance.OnWSkillEvent.AddListener(WSkill);
        PlayerInputManager.Instance.OnESkillEvent.AddListener(ESkill);
    }

    private void OnAttackCursor() => _cursorUIController.SetAttackCursor();
    private void OnLeftClick() => _cursorUIController.SetDefaultCursor();
    private void OnRightClick() => _cursorUIController.SetDefaultCursor();

    public void Init(HPController hpContorller, UnitTeamType teamType)
    {
        if (teamType == UnitTeamType.RedTeam)
        {
            hpContorller.OnChangeHPEvent.AddListener(
                (float maxHp, float currentHP) =>
                _playerScreenHPBarUI.UpdateRedTeamHPBar(maxHp, currentHP)
            );
        }
        else if (teamType == UnitTeamType.BlueTeam)
        {
            hpContorller.OnChangeHPEvent.AddListener(
                (float maxHp, float currentHP) =>
                _playerScreenHPBarUI.UpdateBlueTeamHPBar(maxHp, currentHP)
            );
        }

        hpContorller.OnChangeHPEvent.AddListener(
            (float maxHp, float currentHP) =>
            _playerScreenHUDUI.UpdateHpBar(maxHp, currentHP)
            );

        hpContorller.HPChangeRpc();
    }

    public void SetHUDLevel(int level) => _playerScreenHUDUI.SetLevel(level);
    public void SetHeroPortrait(Sprite image) => _playerScreenHUDUI.SetHeroPortraitImage(image);
    public void SetItemImage(ButtonType button, Sprite sprite) => _playerScreenHUDUI.SetButtonImage(button, sprite);

    public void QSkill()
    {
        Logger.Info("Q");
    }
    public void WSkill()
    {
        Logger.Info("W");
    }
    public void ESkill()
    {
        Logger.Info("E");
    }

    public void EnableRespawnPanel(float respawnTime)
    {
        _respawnWaitPanel.SetActive(true);
        _respawnTimerText.text = Mathf.CeilToInt(respawnTime).ToString();
    }

    public void UpdateRespawnTimer(float remainingSeconds)
    {
        _respawnTimerText.text = Mathf.CeilToInt(remainingSeconds).ToString();
    }

    public void DisableRespawnPanel()
    {
        _respawnWaitPanel.SetActive(false);
    }
}



