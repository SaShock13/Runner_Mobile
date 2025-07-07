
using System;

[System.Serializable]
public class CurrencyCost
{
    public CurrencyType type;
    public string Name => GetName();

    public int amount;
    public CurrencyCost(CurrencyType type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
    private string GetName()
    {
        switch(type)
        {
            case CurrencyType.Coins:
                return "Монеты";
                break;
            case CurrencyType.Diamonds:
                return "Алмазы";
                break;
        }
        return "";
    }

}
