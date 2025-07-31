using UnityEngine;

public class PlayerAnimatorManager 
{
    private Animator _animator;
    private EventBus _eventBus;

    public void Initialize (Animator animator, EventBus eventBus)
    {
        _animator = animator;
        _eventBus = eventBus;
        _eventBus.OnPlayerDamagedEvent += Damaged;
        _eventBus.OnPlayerDeathEvent += Death;
    }


    private void Death()
    {
        _animator.SetBool("Death", true);
    }

    private void Damaged(int arg1, int arg2)
    {
        _animator.SetTrigger("Hit");
        
    }

    public void PlaySlide()
    {
        _animator.SetTrigger("Slide");
    }

    public void PlayJump()
    {
        _animator.SetTrigger("Jump");
    }

     public void PlayHappy()
    {
        _animator.SetTrigger("Happy");
    }



    public void UnSubscribeAll()
    {
        _eventBus.OnPlayerDamagedEvent -= Damaged;
        _eventBus.OnPlayerDeathEvent -= Death;
    }

}
