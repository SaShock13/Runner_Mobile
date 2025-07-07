using Zenject;

public class PlayerWallet 
{
    [Inject] PlayerProgress _playerProgress;
    [Inject] IDataService _dataService;

    public bool IsCanBuy(ShopPrice price)
    {
        foreach (var cost in price.costs)
        {
            if (GetCurrencyAmount(cost.type) - cost.amount < 0) return false;
        }
        return true;
    }

    public void SpendCurrencies(ShopPrice price)
    {
        if (IsCanBuy(price))
        {
            foreach (var cost in price.costs)
            {
                switch (cost.type)
                {
                    case CurrencyType.Coins:
                        _playerProgress.Coins -= cost.amount;
                        break;
                    case CurrencyType.Diamonds:
                        _playerProgress.Diamonds -= cost.amount;
                        break;
                        // Добавьте другие валюты
                }
            }
            _dataService.SavePlayerProgress(_playerProgress);
        }
    }

    private int GetCurrencyAmount(CurrencyType currencyType)
    {
        return currencyType switch
        {
            CurrencyType.Coins => _playerProgress.Coins,
            CurrencyType.Diamonds => _playerProgress.Diamonds,
            _ => 0
        };
    }
}
