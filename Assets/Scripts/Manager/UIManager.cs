using NUnit.Framework.Internal.Commands;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private CursorUIController _cursorUIController;
    [SerializeField] private Canvas _hpUICanvas;
    private GameObject _redTeamHero;
    private GameObject _blueTeamHero;

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

        PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(OnAttackCursor);
        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(OnLeftClick);
        PlayerInputManager.Instance.OnRightClickEvent.AddListener(OnRightClick);
    }

    /// <summary>
    /// 플레이어 별 UI HP Bar 초기화
    /// </summary>
    public void UIInit(UnitTeamType teamType)
    {
        if (teamType == UnitTeamType.RedTeam)
        {
            _hpUICanvas.GetComponent<PlayerScreenHPBarUI>().RedTeamInit(_redTeamHero.GetComponent<HPController>().OnChangeHPEvent);
        }
        else if (teamType == UnitTeamType.BlueTeam)
        {
            _hpUICanvas.GetComponent<PlayerScreenHPBarUI>().BlueTeamInit(_blueTeamHero.GetComponent<HPController>().OnChangeHPEvent);
        }
    }

    /// <summary>
    /// 플레이어 초기화
    /// </summary>
    /// <param name="player"></param>
    /// <param name="teamType"></param>
    public void ObjectInit(GameObject player, UnitTeamType teamType)
    {
        if (teamType == UnitTeamType.RedTeam)
        {
            _redTeamHero = player;
        }
        else if (teamType == UnitTeamType.BlueTeam)
        {
            _blueTeamHero = player;
        }
        else
        {
            Logger.Error("TeamType 정보가 잘못되었습니다.");
        }
    }
}



