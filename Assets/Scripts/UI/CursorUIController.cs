using UnityEngine;

public class CursorUIController : MonoBehaviour
{
    private enum CursorType
    {
        Default,
        Attack,
        Hover
    }

    [SerializeField] private Texture2D _cursorDefault;
    [SerializeField] private Texture2D _cursorAttack;
    [SerializeField] private Texture2D _cursorHover;

    [SerializeField] private LayerMask _unitLayer;

    private CursorType _currentState = CursorType.Default;
    private bool _isHovering = false;

    private void Start()
    {
        SetCursor(CursorType.Default);
    }

    private void Update()
    {
        CheckHoverTarget();
    }

    public void SetDefaultCursor()
    {
        if (_currentState != CursorType.Default)
        {
            SetCursor(CursorType.Default);
            _currentState = CursorType.Default;
        }
    }

    public void SetAttackCursor()
    {
        if (_currentState != CursorType.Attack)
        {
            SetCursor(CursorType.Attack);
            _currentState = CursorType.Attack;
        }
    }

    public void SetHoverCursor()
    {
        if (_currentState != CursorType.Hover)
        {
            SetCursor(CursorType.Hover);
            _currentState = CursorType.Hover;
        }
    }

    private void SetCursor(CursorType type)
    {
        switch (type)
        {
            case CursorType.Attack:
                Cursor.SetCursor(_cursorAttack, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.Hover:
                Cursor.SetCursor(_cursorHover, Vector2.zero, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(_cursorDefault, Vector2.zero, CursorMode.Auto);
                break;
        }
    }

    private UnitController _currentHoverUnit;

    private void CheckHoverTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        _isHovering = Physics.Raycast(ray, out RaycastHit unitHit, 100.0f, _unitLayer);

        if (_isHovering)
        {
            UnitController hitUnit = unitHit.collider.GetComponent<UnitController>();

            if (_currentHoverUnit != hitUnit)
            {
                if (_currentHoverUnit != null)
                {
                    _currentHoverUnit.SetOutline(false);
                }
                _currentHoverUnit = hitUnit;
                _currentHoverUnit.SetOutline(true);
            }

            UnitTeamType teamType = hitUnit.TeamType;
            if (_currentState != CursorType.Attack && teamType != GameManager.Instance.LocalPlayerTeamType)
            {
                SetHoverCursor();
            }
        }
        else
        {
            if (_currentHoverUnit != null)
            {
                _currentHoverUnit.SetOutline(false);
                _currentHoverUnit = null;
            }

            if (_currentState != CursorType.Attack)
            {
                SetDefaultCursor();
            }
        }
    }
}
