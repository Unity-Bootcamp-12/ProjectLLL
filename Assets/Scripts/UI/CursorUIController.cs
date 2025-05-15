using System;
using UnityEngine;
using UnityEngine.Events;

public class CursorUIController : MonoBehaviour
{
    public enum CursorType { Default, Attack, Hover }
    [SerializeField] Texture2D defaultCursor;
    [SerializeField] Texture2D attackCursor;
    [SerializeField] Texture2D hoverCursor;

    [Header("레이어 설정")]
    [SerializeField] private LayerMask _unitLayer;

    [SerializeField] private Camera mainCamera;

    private CursorType _currentState = CursorType.Default;
    private bool _isHovering = false;

    private void Start()
    {
        PlayerInputManager.Instance.OnAttackButtonEvent.AddListener(SetAttackCursor);
        PlayerInputManager.Instance.OnLeftClickEvent.AddListener(SetDefaultCursor);
        PlayerInputManager.Instance.OnRightClickEvent.AddListener(SetDefaultCursor);
        SetCursor(CursorType.Default);
    }

    private void Update()
    {
        CheckHoverTarget();
    }

    private void SetDefaultCursor()
    {
        if (_currentState != CursorType.Default)
        {
            SetCursor(CursorType.Default);
            _currentState = CursorType.Default;
        }
    }

    private void SetAttackCursor()
    {
        if (_currentState != CursorType.Attack)
        {
            SetCursor(CursorType.Attack);
            _currentState = CursorType.Attack;
        }
    }

    private void SetHoverCursor()
    {
        if (_currentState != CursorType.Hover)
        {
            SetCursor(CursorType.Hover);
            _currentState = CursorType.Hover;
        }
    }

    public void SetCursor(CursorType type)
    {
        switch (type)
        {
            case CursorType.Attack:
                Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.Hover:
                Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
        }
    }

    private void CheckHoverTarget()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        _isHovering = Physics.Raycast(ray, out RaycastHit unitHit, Mathf.Infinity, _unitLayer) && unitHit.transform.tag != "Player"; // 같은 팀은 호버상태 안키도록 수정 필요

        if (_isHovering && _currentState != CursorType.Attack)
        {
            SetHoverCursor();
        }
        else if (!_isHovering && _currentState != CursorType.Attack)
        {
            SetDefaultCursor();
        }
    }
}
