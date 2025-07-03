using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    //[System.Serializable]
    //public class SkinData
    //{
    //    public GameObject prefab;
    //    public string skinName;
    //    public int price;
    //    public bool purchased;
    //}

    //public List<SkinData> skins = new List<SkinData>();
    //public Transform skinContainer;
    //public Text infoText;
    //public Button actionButton;

    //private GameObject currentSkinInstance;
    //private int currentIndex;
    //private PlayerData playerData;

    //void Start()
    //{
    //    playerData = LoadPlayerData();
    //    InitializeShop();
    //    ShowSkin(0);
    //}

    //void InitializeShop()
    //{
    //    // Создаем экземпляры всех скинов
    //    foreach (var skin in skins)
    //    {
    //        GameObject instance = Instantiate(skin.prefab, skinContainer);
    //        instance.SetActive(false);
    //        instance.layer = LayerMask.NameToLayer("Skins"); // Для рендер-камеры
    //    }
    //}

    //public void ShowSkin(int index)
    //{
    //    currentIndex = index;
    //    SkinData skin = skins[currentIndex];

    //    // Деактивируем текущую модель
    //    if (currentSkinInstance != null)
    //        currentSkinInstance.SetActive(false);

    //    // Активируем новую модель
    //    currentSkinInstance = skinContainer.GetChild(index).gameObject;
    //    currentSkinInstance.SetActive(true);

    //    // Обновляем UI
    //    infoText.text = $"{skin.skinName}\nЦена: {skin.price} монет";

    //    // Настройка кнопки действия
    //    if (skin.purchased)
    //    {
    //        actionButton.GetComponentInChildren<Text>().text = "Выбрать";
    //        actionButton.interactable = true;
    //    }
    //    else
    //    {
    //        actionButton.GetComponentInChildren<Text>().text = "Купить";
    //        actionButton.interactable = playerData.coins >= skin.price;
    //    }
    //}

    //public void NextSkin() => ShowSkin((currentIndex + 1) % skins.Count);

    //public void PreviousSkin() => ShowSkin((currentIndex - 1 + skins.Count) % skins.Count);

    //public void OnActionButton()
    //{
    //    SkinData skin = skins[currentIndex];

    //    if (skin.purchased)
    //    {
    //        // Выбор скина
    //        playerData.selectedSkin = currentIndex;
    //        SavePlayerData(playerData);
    //        Debug.Log($"Скин {skin.skinName} выбран!");
    //    }
    //    else
    //    {
    //        // Покупка скина
    //        if (playerData.coins >= skin.price)
    //        {
    //            playerData.coins -= skin.price;
    //            skin.purchased = true;
    //            actionButton.GetComponentInChildren<Text>().text = "Выбрать";
    //            SavePlayerData(playerData);
    //        }
    //    }
    //}

    //// Система сохранений (замените на свою реализацию)
    //private PlayerData LoadPlayerData() => new PlayerData();
    //private void SavePlayerData(PlayerData data) { }
}
