using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private PlayerScreenHPBarUI _playerScreenHPBarUI;
    [SerializeField] private PlayerScreenHUDUI _playerScreenHUDUI;
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

        hpContorller.HPChangeRpc();
    }
}



