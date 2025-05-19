using UnityEngine;

public class CursorUIController : MonoBehaviour
{
    //private enum CursorType
    //{
    //    Default,
    //    Attack,
    //    Hover
    //}

    //[SerializeField] private Texture2D _cursorDefault;
    //[SerializeField] private Texture2D _cursorAttack;
    //[SerializeField] private Texture2D _cursorHover;

    //[SerializeField] private LayerMask _unitLayer;

    //private CursorType _currentState = CursorType.Default;
    //private bool _isHovering = false;

    //private void Start()
    //{
    //    PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(SetAttackCursor);
    //    PlayerInputManager.Instance.OnLeftClickEvent.AddListener(SetDefaultCursor);
    //    PlayerInputManager.Instance.OnRightClickEvent.AddListener(SetDefaultCursor);
    //    SetCursor(CursorType.Default);
    //}

    //private void Update()
    //{
    //    CheckHoverTarget();
    //}

    //private void SetDefaultCursor()
    //{
    //    if (_currentState != CursorType.Default)
    //    {
    //        SetCursor(CursorType.Default);
    //        _currentState = CursorType.Default;
    //    }
    //}

    //private void SetAttackCursor()
    //{
    //    if (_currentState != CursorType.Attack)
    //    {
    //        SetCursor(CursorType.Attack);
    //        _currentState = CursorType.Attack;
    //    }
    //}

    //private void SetHoverCursor()
    //{
    //    if (_currentState != CursorType.Hover)
    //    {
    //        SetCursor(CursorType.Hover);
    //        _currentState = CursorType.Hover;
    //    }
    //}

    //public void SetCursor(CursorType type)
    //{
    //    switch (type)
    //    {
    //        case CursorType.Attack:
    //            Cursor.SetCursor(_cursorAttack, Vector2.zero, CursorMode.Auto);
    //            break;
    //        case CursorType.Hover:
    //            Cursor.SetCursor(_cursorHover, Vector2.zero, CursorMode.Auto);
    //            break;
    //        default:
    //            Cursor.SetCursor(_cursorDefault, Vector2.zero, CursorMode.Auto);
    //            break;
    //    }
    //}

    //private void CheckHoverTarget()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    _isHovering = Physics.Raycast(ray, out RaycastHit unitHit, 100.0f, _unitLayer) && unitHit.transform.tag != "Player"; // 같은 팀은 호버상태 안키도록 수정 필요

    //    if (_isHovering && _currentState != CursorType.Attack)
    //    {
    //        SetHoverCursor();
    //    }
    //    else if (!_isHovering && _currentState != CursorType.Attack)
    //    {
    //        SetDefaultCursor();
    //    }
    //}
}
