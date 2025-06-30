using System;
using UnityEngine;
using Zenject;

[System.Serializable]
public class PlayerStats 
{
    public int maxhealth = 10;
    public int currenHealth;
    public float baseRunSpeed = 5f;
    public float currentRunSpeed = 5f;
    public float maxRunSpeed = 50f;
    public float strafeTime = 0.5f;
    public bool IsInvincible = false;
    public int coins;
    public int bonuses;
    public int diamonds = 0;

    public bool IsAlive => currenHealth > 0;

    private EventBus _eventBus;

    [Inject]
    public PlayerStats(EventBus eventBus)
    {
        _eventBus = eventBus;
        _eventBus.OnBonusCollectedEvent += CollectBonus;
        _eventBus.OnCoinCollectedEvent += CollectCoin;
        _eventBus.OnDiamondCollectedEvent += CollectDiamond;
        currenHealth = maxhealth;

        Debug.Log($"PlayerStats ctor {this}");
    }

    public void CollectBonus()
    {
        bonuses++;
        _eventBus.PublishOnBonusAmountChangedEvent(bonuses);
    }
     public void CollectCoin()
    {
        coins++;
        _eventBus.PublishOnCoinsAmountChangedEvent(coins);
    }
     public void CollectDiamond()
    {
        diamonds++;
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
}
