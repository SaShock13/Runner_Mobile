using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using DG.Tweening;
using System.Threading;

[System.Serializable]
public class PlayerStats  : IDisposable
{
    public int maxhealth = 10;
    public int currenHealth;
    public float baseRunSpeed = 5f;
    private float currentRunSpeed = 5f;
    public float maxRunSpeed = 50f;
    public float maxAchivedSpeed = 0f;
    public float maxAchivedDistance = 0f;
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

    //public float Distance { get => distance; set => distance = value; }
    public float CurrentRunSpeed 
    { 
        get => currentRunSpeed; 
        set { 
            currentRunSpeed = value; 
            maxAchivedSpeed = Math.Max(CurrentRunSpeed, maxAchivedSpeed); } 
    }  


    private CancellationTokenSource ctsX2;
    private CancellationTokenSource ctsInvincibility;
    private CancellationTokenSource ctsSpeedBoost;
    private float originalSpeed;
    private bool isSpeedBoostTweenActive;

    private float x2RemainingDuration = 0;
    private float invincibilityRemainingDuration = 0;
    private float speedBoostRemainingDuration = 0;

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
        _eventBus.OnDistanceChangedEvent += DistanceChanged;
        currenHealth = maxhealth;

        DebugUtils.LogEditor($"PlayerStats ctor {this}");
    }

    private void DistanceChanged(float distance)
    {
        maxAchivedDistance  = MathF.Max( maxAchivedDistance, distance );
    }

    private async void SpeedBoostCollected(float duration)
    {
        //speedBoostRemainingDuration += duration;
        //if (!IsSpeedBoosted)
        //{
        //    DebugUtils.LogEditor($"SpeedBoost {this}");
        //    var startspeed = CurrentRunSpeed;
        //    DOTween.To(
        //        () => CurrentRunSpeed,
        //        x => CurrentRunSpeed = x,
        //        CurrentRunSpeed * 2,
        //        2f
        //    ).OnComplete(() => { _eventBus.PublishOnSpeedChangedEvent(CurrentRunSpeed); maxAchivedSpeed = Math.Max(CurrentRunSpeed, maxAchivedSpeed); } ) ;
        //    IsSpeedBoosted = true;

        //    await UniTask.Delay(TimeSpan.FromSeconds(duration));  // todo поправить не всегда корректное отображение скорости в UI
        //    DOTween.To(
        //        () => CurrentRunSpeed,
        //        x => CurrentRunSpeed = x,
        //        startspeed,
        //        2f
        //    ).OnComplete(() => {
        //        IsSpeedBoosted = false;
        //        _eventBus.PublishOnSpeedChangedEvent(CurrentRunSpeed);
        //    });
        //}

        speedBoostRemainingDuration += duration;

        if (IsSpeedBoosted)
        {
            DebugUtils.LogEditor($"SpeedBoost extended: {speedBoostRemainingDuration}s");
            return;
        }

        DebugUtils.LogEditor($"SpeedBoost activated: {duration}s");
        originalSpeed = CurrentRunSpeed;
        IsSpeedBoosted = true;
        ctsSpeedBoost = new CancellationTokenSource();

        isSpeedBoostTweenActive = true;
        DOTween.To(
            () => CurrentRunSpeed,
            x => CurrentRunSpeed = x,
            originalSpeed * 2f,
            1f
        ).OnComplete(() => {
            isSpeedBoostTweenActive = false;
            _eventBus.PublishOnSpeedChangedEvent(CurrentRunSpeed);
            maxAchivedSpeed = Math.Max(CurrentRunSpeed, maxAchivedSpeed);
        });

        try
        {
            while (speedBoostRemainingDuration > 0f)
            {
                float step = Mathf.Min(speedBoostRemainingDuration, 0.1f);
                await UniTask.Delay(TimeSpan.FromSeconds(step), cancellationToken: ctsSpeedBoost.Token);
                speedBoostRemainingDuration -= step;

                if (speedBoostRemainingDuration <= 0f) break;
            }
        }
        catch (OperationCanceledException)
        {
            DebugUtils.LogEditor("SpeedBoost was cancelled");
        }
        finally
        {
            if (!ctsSpeedBoost.IsCancellationRequested)
            {
                await UniTask.WaitUntil(() => !isSpeedBoostTweenActive);

                DOTween.To(
                    () => CurrentRunSpeed,
                    x => CurrentRunSpeed = x,
                    originalSpeed,
                    1f
                ).OnComplete(() => {
                    IsSpeedBoosted = false;
                    _eventBus.PublishOnSpeedChangedEvent(CurrentRunSpeed);
                });
            }

            ctsSpeedBoost?.Dispose();
            ctsSpeedBoost = null;
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
        CurrentRunSpeed = baseRunSpeed;
        maxAchivedSpeed = 0;
        maxAchivedDistance = 0;
        strafeTime = 0.5f;
        speedMultiplyer = 1.5f;
        bonusMultiplyer = 1f;
        _eventBus.PublishOnPlayerDamaged(maxhealth, currenHealth);
        _eventBus.PublishOnSpeedChangedEvent(CurrentRunSpeed);
        _eventBus.PublishOnBonusAmountChangedEvent(bonuses);
        _eventBus.PublishOnCoinsAmountChangedEvent(coins);
        _eventBus.PublishOnDiamondAmountChangedEvent(diamonds);
    }

    public string ReturnStatistics()
    {
        return $"Собрано : \n" +
            $"Звезд : {bonuses}\n" +
            $"Монет : {coins}\n" +
            $"Алмазов : {diamonds}\n" +
            $"Пробежал {maxAchivedDistance} метров \n" +
            $"Максимальная скорость {(int)maxAchivedSpeed} км/ч";
    }

    private async void MultiplyerX2Collected(float duration)
    {

        //DebugUtils.LogEditor($"MultiplyerX2 {this}");
        //IsMultilier = true;
        //bonusMultiplyer = 2f;
        //await UniTask.Delay(TimeSpan.FromSeconds(duration));
        //bonusMultiplyer = 1f;
        //IsMultilier = false;


        DebugUtils.LogEditor($"MultiplyerX2 {this}");

        // Добавляем новую длительность к оставшемуся времени
        x2RemainingDuration += duration;

        if (IsMultilier)
        {
            DebugUtils.LogEditor($"Bonus extended. Remaining time: {x2RemainingDuration}");
            return;
        }

        IsMultilier = true;
        bonusMultiplyer = 2f;

        // Создаем новый CancellationTokenSource
        ctsX2 = new CancellationTokenSource();

        try
        {
            // Обрабатываем эффект пока есть оставшееся время
            while (x2RemainingDuration > 0f)
            {
                // Ждем минимальный шаг времени (например, 0.1 сек)
                float step = Mathf.Min(x2RemainingDuration, 0.1f);
                await UniTask.Delay(TimeSpan.FromSeconds(step), cancellationToken: ctsX2.Token);

                x2RemainingDuration -= step;

                // Если время закончилось - выключаем эффект
                if (x2RemainingDuration <= 0f)
                {
                    bonusMultiplyer = 1f;
                    IsMultilier = false;
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Эффект был отменен (например, при уничтожении объекта)
            DebugUtils.LogEditor("Multiplyer effect was cancelled");
        }
        finally
        {
            ctsX2?.Dispose();
            ctsX2 = null;
        }
    }

    private async void InvincibilityCollected(float duration)
    {
        //DebugUtils.LogEditor($"IsInvincible {this}");
        //IsInvincible = true;
        //await UniTask.Delay(TimeSpan.FromSeconds(duration));
        //IsInvincible = false;
        DebugUtils.LogEditor($"InvincibilityCollected {this}");
        invincibilityRemainingDuration += duration;

        if (IsInvincible)
        {
            DebugUtils.LogEditor($"Bonus extended. Remaining time: {invincibilityRemainingDuration}");
            return;
        }

        IsInvincible = true;

        ctsInvincibility = new CancellationTokenSource();

        try
        {
            while (invincibilityRemainingDuration > 0f)
            {
                float step = Mathf.Min(invincibilityRemainingDuration, 0.1f);
                await UniTask.Delay(TimeSpan.FromSeconds(step), cancellationToken: ctsInvincibility.Token);

                invincibilityRemainingDuration -= step;

                if (invincibilityRemainingDuration <= 0f)
                {
                    IsInvincible = false;
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            DebugUtils.LogEditor("Invincibility effect was cancelled");
        }
        finally
        {
            ctsInvincibility?.Dispose();
            ctsInvincibility = null;
        }
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

    public void ResetSpeed()
    {
        CurrentRunSpeed = baseRunSpeed;
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
