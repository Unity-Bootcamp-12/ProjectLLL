using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private CursorUIController _cursorUIController;
    private PlayerScreenHPBarUI _playerScreenHPBarUI;
    [SerializeField] private Canvas _hpUICanvas;

    private void OnAttackCursor() => _cursorUIController.SetAttackCursor();
    private void OnLeftClick() => _cursorUIController.SetDefaultCursor();
    private void OnRightClick() => _cursorUIController.SetDefaultCursor();

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
    }

    private void Start()
    {
        _cursorUIController = GetComponent<CursorUIController>();
        _playerScreenHPBarUI = GetComponent<PlayerScreenHPBarUI>();

        PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(OnAttackCursor);
        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftClick);
        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightClick);
    }

    /// <summary>
    /// 플레이어 별 UI HP Bar 초기화
    /// </summary>
    public void UIInit(UnityEvent<float, float> unityEvent, UnitTeamType teamType)
    {
        if (teamType == UnitTeamType.RedTeam)
        {
            _playerScreenHPBarUI.RedTeamInit(unityEvent);
        }
        else if (teamType == UnitTeamType.BlueTeam)
        {
            _playerScreenHPBarUI.BlueTeamInit(unityEvent);
        }
    }
}



