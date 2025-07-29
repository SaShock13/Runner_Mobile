using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SkinShop : MonoBehaviour
{
    [SerializeField] private List<SkinData> skins = new List<SkinData>();
    [SerializeField] private Transform skinContainer;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button actionButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] private TMP_Text shopCoinsAmount;
    [SerializeField] private TMP_Text shopDiamondsAmount;

    [Inject] Player _player;
    [Inject] PlayerWallet _playerWallet;
    [Inject] FirebaseRemoteConfigManager _remoteConfigManager;
    [Inject] PlayerProgress _progress;


    private GameObject currentSkinInstance;
    private int currentItemIndex;

    void Start()
    {        
        InitializeShop();
        ShowSkin(0);
        leftButton.onClick.AddListener(PreviousSkin);
        rightButton.onClick.AddListener(NextSkin);
        actionButton.onClick.AddListener(OnActionButton);
    }

    void InitializeShop()
    {
        foreach (var skin in skins)
        {
            GameObject instance = Instantiate(skin.prefab, skinContainer);
            instance.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            instance.SetActive(false);
            instance.layer = LayerMask.NameToLayer("Skins"); // Для рендер-камеры
        }
        foreach (var skin in skins)
        {
            skin.price.costs.Clear();
        }
        if (_remoteConfigManager.GetIntValue("skin1_CoinPrice") > 0)
        { skins[1].price.costs.Add(new CurrencyCost(CurrencyType.Coins, _remoteConfigManager.GetIntValue("skin1_CoinPrice"))); }
        if (_remoteConfigManager.GetIntValue("skin1_DiamondPrice") > 0)
        { skins[1].price.costs.Add(new CurrencyCost(CurrencyType.Diamonds, _remoteConfigManager.GetIntValue("skin1_DiamondPrice"))); }
        if (_remoteConfigManager.GetIntValue("skin2_CoinPrice") > 0)
        { skins[2].price.costs.Add(new CurrencyCost(CurrencyType.Coins, _remoteConfigManager.GetIntValue("skin2_CoinPrice"))); }
        if (_remoteConfigManager.GetIntValue("skin2_DiamondPrice") > 0)
        { skins[2].price.costs.Add(new CurrencyCost(CurrencyType.Diamonds, _remoteConfigManager.GetIntValue("skin2_DiamondPrice"))); }
        UpdateResourcesView();
    }

    private void UpdateResourcesView()
    {
        shopCoinsAmount.text = _progress.Coins.ToString();
        shopDiamondsAmount.text = _progress.Diamonds.ToString();
    }

    public void ShowSkin(int index)
    {
        currentItemIndex = index;
        SkinData skin = skins[currentItemIndex];
        if (currentSkinInstance != null)
            currentSkinInstance.SetActive(false);
        currentSkinInstance = skinContainer.GetChild(index).gameObject;
        currentSkinInstance.SetActive(true);
        infoText.text = $"{skin.skinName}\nЦена: ";
        foreach (var cost in skin.price.costs)
        {
            infoText.text += $" \n{cost.Name} - {cost.amount}";
        }

        if (skin.purchased)
        {
            actionButton.GetComponentInChildren<TMP_Text>().text = "Выбрать";
            actionButton.interactable = true;
        }
        else
        {
            actionButton.GetComponentInChildren<TMP_Text>().text = "Купить";
        }
    }

    public void NextSkin() => ShowSkin((currentItemIndex + 1) % skins.Count);

    public void PreviousSkin() => ShowSkin((currentItemIndex - 1 + skins.Count) % skins.Count);

    public void OnActionButton()
    {
        SkinData skin = skins[currentItemIndex];

        if (skin.purchased)
        {
            DebugUtils.LogEditor($"skin.purchased {skin.purchased}");
            _player.SetSkinPrefab(skin.prefab);
            DebugUtils.LogEditor($"Скин {skin.skinName} выбран!");
        }
        else
        {
            if(_playerWallet.IsCanBuy(skin.price))
            {
                _playerWallet.SpendCurrencies(skin.price);
                skin.purchased = true;
                DebugUtils.LogEditor($"skin.purchased {skin.purchased}");

                actionButton.GetComponentInChildren<TMP_Text>().text = "Выбрать";
                UpdateResourcesView();
                //actionButton.interactable = true;
            }
            else DebugUtils.LogEditor($"Not enough currencies {this}");
            
        }
    }
}
