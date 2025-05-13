using UnityEngine;
using UnityEngine.Events;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public UnityEvent OnLeftClickEvent;
    public UnityEvent OnRightClickEvent;

    public UnityEvent OnAttackButtonEvent;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            OnLeftClickEvent?.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnRightClickEvent?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnAttackButtonEvent?.Invoke();
        }
    }
}
