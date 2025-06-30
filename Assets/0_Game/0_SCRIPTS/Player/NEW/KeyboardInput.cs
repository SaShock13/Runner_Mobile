using System;
using UnityEngine;

public class KeyboardInput : MonoBehaviour , IInput
{
    public event Action OnSwipeLeft;
    public event Action OnSwipeRight;
    public event Action OnSwipeUp;
    public event Action OnSwipeDown;

    void Update()
    {
        ReadKeyboardInput();
    }


    /// <summary>
    /// For pc test
    /// </summary>
    private void ReadKeyboardInput()
    {
        //xInput = Input.GetAxis("Horizontal");


        if (Input.GetKeyDown(KeyCode.A))
        {
            OnSwipeLeft?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            OnSwipeRight?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            OnSwipeUp?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            OnSwipeDown?.Invoke();
        }



    }
}
