using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SkinShop : MonoBehaviour
{
    [SerializeField] private List<SkinData> allSkins = new List<SkinData>();
    private List<SkinData> shopSkins = new List<SkinData>();
    [SerializeField] private Transform skinContainer;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button actionButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] private TMP_Text shopCoinsAmount;
    [SerializeField] private TMP_Text shopDiamondsAmount;
    [SerializeField] private TMP_Text notEnoughtText;

    [Inject] Player _player;
    [Inject] PlayerWallet _playerWallet;
    [Inject] FirebaseRemoteConfigManager _remoteConfigManager;
    [Inject] PlayerProgress _progress;
    [Inject] IDataService _dataService;
    [Inject] SoundManager _soundManager;


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
        foreach (var skin in allSkins)
        {
            if (_progress.UnlockedSkins.Contains(skin.skinName)) skin.purchased = true;
            skin.price.costs.Clear();
            GameObject instance = Instantiate(skin.prefab, skinContainer);
            instance.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            instance.SetActive(false);
            instance.layer = LayerMask.NameToLayer("Skins"); // Для рендер-камеры
        }

        if (_remoteConfigManager.GetIntValue("skin1_CoinPrice") > 0)
        { allSkins[1].price.costs.Add(new CurrencyCost(CurrencyType.Coins, _remoteConfigManager.GetIntValue("skin1_CoinPrice"))); }
        if (_remoteConfigManager.GetIntValue("skin1_DiamondPrice") > 0)
        { allSkins[1].price.costs.Add(new CurrencyCost(CurrencyType.Diamonds, _remoteConfigManager.GetIntValue("skin1_DiamondPrice"))); }
        if (_remoteConfigManager.GetIntValue("skin2_CoinPrice") > 0)
        { allSkins[2].price.costs.Add(new CurrencyCost(CurrencyType.Coins, _remoteConfigManager.GetIntValue("skin2_CoinPrice"))); }
        if (_remoteConfigManager.GetIntValue("skin2_DiamondPrice") > 0)
        { allSkins[2].price.costs.Add(new CurrencyCost(CurrencyType.Diamonds, _remoteConfigManager.GetIntValue("skin2_DiamondPrice"))); }
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
        SkinData skin = allSkins[currentItemIndex];
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

    public void NextSkin() => ShowSkin((currentItemIndex + 1) % allSkins.Count);

    public void PreviousSkin() => ShowSkin((currentItemIndex - 1 + allSkins.Count) % allSkins.Count);

    public void OnActionButton()
    {
        SkinData skin = allSkins[currentItemIndex];

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
                _progress.UnlockedSkins.Add(skin.skinName);
                _dataService.SavePlayerProgress(_progress);
                DebugUtils.LogEditor($"skin.purchased {skin.purchased}");

                actionButton.GetComponentInChildren<TMP_Text>().text = "Выбрать";
                UpdateResourcesView();
                //actionButton.interactable = true;
            }
            else NotEnoughtMessage();

        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Y))
        {
            NotEnoughtMessage();
        }

    }

    private void NotEnoughtMessage()
    {
        DebugUtils.LogEditor($"Not enough currencies {this}");
        notEnoughtText.color = new Color(notEnoughtText.color.r, notEnoughtText.color.g, notEnoughtText.color.b, 0);
        notEnoughtText.transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        // Одновременное появление и масштабирование
        sequence.Join(notEnoughtText.DOFade(1f, 0.8f).OnComplete(() => _soundManager.PlaySFX(Sounds.wrong)));
        sequence.Join(notEnoughtText.transform.DOScale(1.1f, 0.5f).SetEase(Ease.OutBack));

        // Минимальная вибрация
        sequence.Append(notEnoughtText.transform.DOShakePosition(0.3f, new Vector3(5, 5, 0)));

        // Исчезновение с уменьшением
        sequence.Append(notEnoughtText.DOFade(0f, 0.7f).SetDelay(0.5f));
        sequence.Join(notEnoughtText.transform.DOScale(0.8f, 0.7f).SetEase(Ease.InQuad));

    }
}
