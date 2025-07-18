using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using DG.Tweening;

[System.Serializable]
public class PlayerStats  : IDisposable
{
    public int maxhealth = 10;
    public int currenHealth;
    public float baseRunSpeed = 5f;
    public float currentRunSpeed = 5f;
    public float maxRunSpeed = 50f;
    public float strafeTime = 0.5f;
    public bool IsInvincible = false;
    public bool IsSpeedBoosted = false;
    public bool IsMultilier = false;
    public int coins;
    public int bonuses;
    public int diamonds = 0;
    private float speedMultiplyer = 1.5f;
    private float bonusMultiplyer = 1f;
    private EventBus _eventBus;
    public bool IsAlive => currenHealth > 0;

    [Inject]
    public PlayerStats(EventBus eventBus)
    {
        _eventBus = eventBus;
        _eventBus.OnBonusCollectedEvent += CollectBonus;
        _eventBus.OnCoinCollectedEvent += CollectCoin;
        _eventBus.OnDiamondCollectedEvent += CollectDiamond;
        _eventBus.OnInvincibilityCollectedEvent += InvincibilityCollected;
        _eventBus.OnMultiplyerX2CollectedEvent += MultiplyerX2Collected;
        _eventBus.OnSpeedBoostCollectedEvent += SpeedBoostCollected;
        currenHealth = maxhealth;

        DebugUtils.LogEditor($"PlayerStats ctor {this}");
    }
    
    private async void SpeedBoostCollected(float duration)
    {
        if (!IsSpeedBoosted)
        {
            DebugUtils.LogEditor($"SpeedBoost {this}");
            var startspeed = currentRunSpeed;
            DOTween.To(
                () => currentRunSpeed,
                x => currentRunSpeed = x,
                currentRunSpeed * 2, 
                2f    
            ).OnComplete(()=> _eventBus.PublishOnSpeedChangedEvent(currentRunSpeed) );
            IsSpeedBoosted = true;
            
            await UniTask.Delay(TimeSpan.FromSeconds(duration));  // todo поправить не всегда корректное отображение скорости в UI
            DOTween.To(
                () => currentRunSpeed,
                x => currentRunSpeed = x,
                startspeed,
                2f
            ).OnComplete(() => {
                IsSpeedBoosted = false;
                _eventBus.PublishOnSpeedChangedEvent(currentRunSpeed);
            });
        }
    }
    public void ResetStats()
    {
        currenHealth = maxhealth;
        coins = 0;
        diamonds = 0;
        bonuses = 0;
        IsInvincible = false;
        IsSpeedBoosted = false;
        IsMultilier = false;
        currentRunSpeed = baseRunSpeed;
        strafeTime = 0.5f;
        speedMultiplyer = 1.5f;
        bonusMultiplyer = 1f;
        _eventBus.PublishOnPlayerDamaged(maxhealth, currenHealth);
        _eventBus.PublishOnSpeedChangedEvent(currentRunSpeed);
        _eventBus.PublishOnBonusAmountChangedEvent(bonuses);
        _eventBus.PublishOnCoinsAmountChangedEvent(coins);
        _eventBus.PublishOnDiamondAmountChangedEvent(diamonds);
    }

    private async void MultiplyerX2Collected(float duration)
    {
        DebugUtils.LogEditor($"MultiplyerX2 {this}");
        IsMultilier = true;
        bonusMultiplyer = 2f;
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        bonusMultiplyer = 1f;
        IsMultilier = false;
    }

    private async void InvincibilityCollected(float duration)
    {
        DebugUtils.LogEditor($"IsInvincible {this}");
        IsInvincible = true;
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        IsInvincible = false;
    }

    public void CollectBonus()
    {
        bonuses += (int)(1 * bonusMultiplyer);
        _eventBus.PublishOnBonusAmountChangedEvent(bonuses);
    }
     public void CollectCoin()
    {
        coins += (int)(1 * bonusMultiplyer);
        _eventBus.PublishOnCoinsAmountChangedEvent(coins);
    }
     public void CollectDiamond()
    {
        diamonds += (int)(1 * bonusMultiplyer);
        _eventBus.PublishOnDiamondAmountChangedEvent(diamonds);
    }
    public void TakeDamage(int damage)
    {
        if (damage > 0 )
        {
            currenHealth -= damage; 
            if (currenHealth <= 0)
            {
                TakeDeath();
            }
            else
            {
                _eventBus.PublishOnPlayerDamaged(maxhealth,currenHealth);
            }
        }
    }

    private void TakeDeath()
    {
        _eventBus.PublishOnPlayerDeathEvent();
    }

    public void IncreaseSpeed(float increaseAmount)
    {
        currentRunSpeed = Mathf.Max( increaseAmount + currentRunSpeed,maxRunSpeed );
    }

    public void ResetSpeed()
    {
        currentRunSpeed = baseRunSpeed;
    }

    public void Dispose()
    {
        _eventBus.OnBonusCollectedEvent -= CollectBonus;
        _eventBus.OnCoinCollectedEvent -= CollectCoin;
        _eventBus.OnDiamondCollectedEvent -= CollectDiamond;
        _eventBus.OnInvincibilityCollectedEvent -= InvincibilityCollected;
        _eventBus.OnMultiplyerX2CollectedEvent -= MultiplyerX2Collected;
        _eventBus.OnSpeedBoostCollectedEvent -= SpeedBoostCollected;
    }
}
