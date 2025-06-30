using System;

public interface IInput 
{
    public event Action OnSwipeLeft;
    public event Action OnSwipeRight;
    public event Action OnSwipeUp;
    public event Action OnSwipeDown;
}
