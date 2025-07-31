using System;
using UnityEngine;

public class EventBus 
{
    public event Action OnMenuRequestEvent;
    public event Action OnMenuEvent;
    public event Action<int,int> OnPlayerDamagedEvent;
    public event Action OnPlayerDeathEvent;
    public event Action OnProgressLoadedEvent;
    public event Action OnRemoteConfigLoadedEvent;
    public event Action OnGameOverEvent;
    public event Action OnGameResetEvent;
    public event Action OnGameRestartRequestEvent;
    public event Action OnPlayerProgressResetRequestEvent;
    public event Action OnPlayerProgressResetEvent;
    public event Action OnGameStartRequestEvent;
    public event Action OnTutorialFinishedEvent;
    public event Action OnGameStartEvent;
    public event Action OnGameDifficultyIncreasedEvent;
    public event Action OnBonusCollectedEvent;
    public event Action OnCoinCollectedEvent;
    public event Action OnDiamondCollectedEvent;
    public event Action<float> OnSpeedBoostCollectedEvent;
    public event Action<float> OnMultiplyerX2CollectedEvent;
    public event Action<float> OnInvincibilityCollectedEvent;
    public event Action<float> OnInvincibilityEffectEvent;
    public event Action<int> OnBonusAmountChangedEvent;
    public event Action<int> OnCoinsAmountChangedEvent;
    public event Action<int> OnDiamondAmountChangedEvent;
    public event Action<float> OnSpeedChangedEvent;
    public event Action OnRequestPauseEvent;
    public event Action OnPauseEvent;
    public event Action OnResumeEvent;

    public event Action<float> OnDistanceChangedEvent;

    public void PublishOnPlayerDamaged(int max, int current)
    {
        OnPlayerDamagedEvent?.Invoke(max,current);
    }
    public void PublishOnPlayerDeathEvent()
    {
        OnPlayerDeathEvent?.Invoke();
    }
    public void PublishOnGameRestartRequestEvent()
    {
        OnGameRestartRequestEvent?.Invoke();
    }
    public void PublishOnGameResetEvent()
    {
        OnGameResetEvent?.Invoke();
    }
     public void PublishOnPlayerProgressResetRequestEvent()
    {
        OnPlayerProgressResetRequestEvent?.Invoke();
    }
     public void PublishOnPlayerProgressResetEvent()
    {
        OnPlayerProgressResetEvent?.Invoke();
    }
    
    public void PublishOnGameStartRequestEvent()
    {
        OnGameStartRequestEvent?.Invoke();
    }
    public void PublishOnGameStartEvent()
    {
        OnGameStartEvent?.Invoke();
    }
    public void PublishOnTutorialFinishedEvent()
    {
        OnTutorialFinishedEvent?.Invoke();
    }

    public void PublishOnGameOverEvent()
    {
        OnGameOverEvent?.Invoke();
    } 
    public void PublishOnGameDifficultyIncreasedEvent()
    {
        OnGameDifficultyIncreasedEvent?.Invoke();
    }
    public void PublishOnBonusCollectedEvent()
    {
        OnBonusCollectedEvent?.Invoke();
    }
    public void PublishOnCoinCollectedEvent()
    {
        OnCoinCollectedEvent?.Invoke();
    }
    public void PublishOnSpeedBoostCollectedEvent(float duration)
    {
        OnSpeedBoostCollectedEvent?.Invoke(duration);
    }
    public void PublishOnMultiplyerX2CollectedEvent(float duration)
    {
        OnMultiplyerX2CollectedEvent?.Invoke(duration);
    }
    public void PublishOnInvincibilityCollectedEvent(float duration)
    {
        OnInvincibilityCollectedEvent?.Invoke(duration);
    }
    public void PublishOnDiamondCollectedEventt()
    {
        OnDiamondCollectedEvent?.Invoke();
    }
    public void PublishOnBonusAmountChangedEvent(int bonusAmount)
    {
        OnBonusAmountChangedEvent?.Invoke(bonusAmount);
    } 
    public void PublishOnCoinsAmountChangedEvent(int coinsAmount)
    {
        OnCoinsAmountChangedEvent?.Invoke(coinsAmount);
    }
    public void PublishOnDiamondAmountChangedEvent(int coinsAmount)
    {
        OnDiamondAmountChangedEvent?.Invoke(coinsAmount);
    }
    public void PublishOnSpeedChangedEvent(float speed)
    {
        OnSpeedChangedEvent?.Invoke(speed);
    } 
    
    public void PublishOnRequestPauseEvent()
    {
        OnRequestPauseEvent?.Invoke();
    }
    public void PublishOnMenuRequestEvent()
    {
        OnMenuRequestEvent?.Invoke();
    }
    public void PublishOnMenuEvent()
    {
        OnMenuEvent?.Invoke();
    }

    public void PublishOProgressLoadedEvent()
    {
        OnProgressLoadedEvent?.Invoke();
    }
    public void PublishOnRemotweConfigLoadedEvent()
    {
        OnRemoteConfigLoadedEvent?.Invoke();
    }
    public void PublishOnPauseEvent()
    {
        OnPauseEvent?.Invoke();
    }
    public void PublishOnResumeEvent()
    {
        OnResumeEvent?.Invoke();
    }

    public void PublishOnDistanceChangedEvent(float distance)
    {
        OnDistanceChangedEvent?.Invoke(distance);
    }

    public void PublishOnInvincibilityEffectEvent(float invincibilityRemainingDuration)
    {
        OnInvincibilityEffectEvent.Invoke(invincibilityRemainingDuration);
    }
}
