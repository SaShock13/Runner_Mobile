using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Shop : MonoBehaviour
{
    [System.Serializable]
    public class SkinData
    {
        public GameObject prefab;
        public string skinName;
        public int price;
        public bool purchased;
    }

    [SerializeField] private List<SkinData> skins = new List<SkinData>();
    [SerializeField] private Transform skinContainer;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button actionButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Inject] Player _player;

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
        // Создаем экземпляры всех скинов
        foreach (var skin in skins)
        {
            GameObject instance = Instantiate(skin.prefab, skinContainer);
            instance.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            instance.SetActive(false);
            instance.layer = LayerMask.NameToLayer("Skins"); // Для рендер-камеры
        }
    }

    public void ShowSkin(int index)
    {
        currentItemIndex = index;
        SkinData skin = skins[currentItemIndex];

        // Деактивируем текущую модель
        if (currentSkinInstance != null)
            currentSkinInstance.SetActive(false);

        // Активируем новую модель
        currentSkinInstance = skinContainer.GetChild(index).gameObject;
        currentSkinInstance.SetActive(true);

        // Обновляем UI
        infoText.text = $"{skin.skinName}\nЦена: {skin.price} монет";

        // Настройка кнопки действия
        if (skin.purchased)
        {
            actionButton.GetComponentInChildren<TMP_Text>().text = "Выбрать";
            actionButton.interactable = true;
        }
        else
        {
            actionButton.GetComponentInChildren<TMP_Text>().text = "Купить";
            //actionButton.interactable = playerData.coins >= skin.price;
        }
    }

    public void NextSkin() => ShowSkin((currentItemIndex + 1) % skins.Count);

    public void PreviousSkin() => ShowSkin((currentItemIndex - 1 + skins.Count) % skins.Count);

    public void OnActionButton()
    {
        SkinData skin = skins[currentItemIndex];

        if (skin.purchased)
        {
            // Выбор скина
            //playerData.selectedSkin = currentIndex;
            //SavePlayerData(playerData);

            Debug.Log($"skin.purchased {skin.purchased}");
            _player.SetSkinPrefab(skin.prefab);
            Debug.Log($"Скин {skin.skinName} выбран!");
        }
        else
        {
            Debug.Log($"skin.purchased {skin.purchased}");
            // Покупка скина
            //if (playerData.coins >= skin.price)
            //{
            //    playerData.coins -= skin.price;
            //    skin.purchased = true;
            //    actionButton.GetComponentInChildren<Text>().text = "Выбрать";
            //    SavePlayerData(playerData);
            //}
        }

    }

    //// Система сохранений (замените на свою реализацию)
    //private PlayerData LoadPlayerData() => new PlayerData();
    //private void SavePlayerData(PlayerData data) { }
}
