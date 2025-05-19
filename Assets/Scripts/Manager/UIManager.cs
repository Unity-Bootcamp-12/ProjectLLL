using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private CursorUIController _cursorUIController;
    private PlayerScreenHPBarUI _playerScreenHPBarUI;
    [SerializeField] private Canvas _hpUICanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _cursorUIController = GetComponent<CursorUIController>();
            _playerScreenHPBarUI = _hpUICanvas.GetComponent<PlayerScreenHPBarUI>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(OnAttackCursor);
        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftClick);
        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightClick);
    }

    private void OnAttackCursor() => _cursorUIController.SetAttackCursor();
    private void OnLeftClick() => _cursorUIController.SetDefaultCursor();
    private void OnRightClick() => _cursorUIController.SetDefaultCursor();

    public void UIInit(HPController hpContorller, UnitTeamType teamType)
    {
        hpContorller.OnChangeHPEvent.AddListener(UpdateTeamHPBar);
        UpdateTeamHPBar(teamType, hpContorller.GetMaxHP(), hpContorller.GetCurrentHP());
    }

    private void UpdateTeamHPBar(UnitTeamType teamType, float maxHP, float currentHP)
    {
        _playerScreenHPBarUI.UpdateTeamHP(teamType, maxHP, currentHP);
    }
}



