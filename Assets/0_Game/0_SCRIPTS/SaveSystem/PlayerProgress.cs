using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Serializable]
public class PlayerProgress 
{
    public string Name ;
    public int HighScore;
    public int Coins;
    public int Diamonds;
    public bool IsFirstTime;
    public List<string> UnlockedSkins = new List<string>();
    public string EquippedSkin;
    public DateTime LastSaveTime;
    [Inject] private EventBus _eventBus;


    public PlayerProgress()
    {
        HighScore = 500;
        Coins = 0;
        Diamonds = 0;
        UnlockedSkins.Add("BoySkin");
        EquippedSkin = "BoySkin";
        LastSaveTime = DateTime.UtcNow;
        IsFirstTime = true;
        Name = "";
    }

    public void CopyTo(PlayerProgress other)
    {
        other.HighScore = HighScore;
        other.Coins = Coins;
        other.Diamonds = Diamonds;
        other.EquippedSkin = EquippedSkin;
        other.LastSaveTime = LastSaveTime;
        other.Name = Name;
        other.IsFirstTime = IsFirstTime;
        foreach (var item in UnlockedSkins)
        {
            other.UnlockedSkins.Add(item);
        }
    }

    public void SpendCoin( int spendAmount)
    {
        if (spendAmount >0 && IsEnoughCoins(spendAmount))
        {
            Coins -= spendAmount;
            _eventBus.PublishOnCoinsAmountChangedEvent( Coins );
        }
    }

    public void AddCoin( int addAmount)
    {
        if (addAmount > 0 )
        {
            Coins += addAmount;
            _eventBus.PublishOnCoinsAmountChangedEvent( Coins );
        }
    }

    public bool IsEnoughCoins(int spendAmount)
    {
        return (Diamonds - spendAmount) > 0;
    }

    
    public void SpendDiamond( int spendAmount)
    {
        if (spendAmount >0 && IsEnoughDiamond(spendAmount))
        {
            Diamonds -= spendAmount;
            _eventBus.PublishOnDiamondAmountChangedEvent(Diamonds);
        }
    }

    public void AddDiamond( int addAmount)
    {
        if (addAmount > 0 )
        {
            Diamonds += addAmount;
            _eventBus.PublishOnDiamondAmountChangedEvent(Diamonds);
        }
    }

    public bool IsEnoughDiamond(int spendAmount)
    {
        return (Diamonds - spendAmount) > 0;
    }
}
