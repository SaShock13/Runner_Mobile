using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class MobileInput: IInput
{
    PlayerMovement movement;
    PlayerHealth health;
    public bool isAutoRun = false;
    private float xInput, yInput;
    private Joystick _joystick;
    [Header("Настройки свайпа")]
    [Tooltip("Минимальная длина в пикселях, чтобы считать это свайпом")]
    private float minSwipeDistance = 50f;
    public event Action OnSwipeLeft;
    public event Action OnSwipeRight;
    public event Action OnSwipeUp;
    public event Action OnSwipeDown;
    public event Action OnSwipe;
    public event Action OnTouchEnd;
    public event Action OnTouchStart;
    public static event Action<string> OnMessage;
    private InputAction touchAction;
    private InputAction TouchPosAction;
    private Vector2 startPos;
    private bool swipeInProgress;
    private Vector2 CurrentPosition => TouchPosAction.ReadValue<Vector2>(); 

    [Inject]
    public void Construct(Joystick joystick)
    {
        _joystick = joystick;        
    }

    public MobileInput()
    {
        touchAction = InputSystem.actions.FindAction("Touch");
        TouchPosAction = InputSystem.actions.FindAction("TouchPos");
        touchAction.performed += ctx => StartSwipe(ctx);
        touchAction.canceled += ctx => EndSwipe(ctx); 
    }

    private void EndSwipe(InputAction.CallbackContext ctx)
    {
        OnTouchEnd?.Invoke();
        Debug.Log($"EndSwipe {this}");
        if (!swipeInProgress) return;
        swipeInProgress = false;

        Vector2 endPos = TouchPosAction.ReadValue<Vector2>(); ;
        Vector2 delta = endPos - startPos;

        if (delta.magnitude < minSwipeDistance)
        {
            OnMessage?.Invoke($" CurrentPosition {CurrentPosition}   delta {delta} startPos {startPos} endPos {endPos}");
            return;
        }
            OnSwipe?.Invoke();
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0) OnSwipeRight?.Invoke();
            else OnSwipeLeft?.Invoke();
        }
        else
        {
            if (delta.y > 0) OnSwipeUp?.Invoke();
            else OnSwipeDown?.Invoke();
        }
    }

    private void StartSwipe(InputAction.CallbackContext ctx)
    {
        OnTouchStart?.Invoke();
        Debug.Log($"End Swipe {this}");
        swipeInProgress = true;
        startPos = CurrentPosition;
    }   
}
