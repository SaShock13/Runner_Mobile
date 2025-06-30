using UnityEngine;
using Zenject;

public class TEstSave : MonoBehaviour
{
    [Inject] private IDataService _dataManager;
    [Inject] private PlayerProgress _progress;
    [Inject] private EventBus _eventBus;

    public void Awake()
    {
        var progress = _dataManager.LoadPlayerProgress(); 
        progress.CopyTo(_progress);
        //_progress .HighScore = 0;
        _eventBus.PublishOProgressLoadedEvent();
        Debug.Log($"Загружен Name: {_progress.Name} \n  ");
        Debug.Log($"Загружен рекорд: {_progress.HighScore} \n  ");
        Debug.Log($"Загружен Coins: {_progress.Coins} \n  ");
        Debug.Log($"Загружен EquippedSkin: {_progress.EquippedSkin} \n  ");
        //_dataManager.DeleteAllData();
        //_progress.HighScore = 0;
        //_progress.Coins = 0;
        //_progress.Diamonds = 0;
        //_progress.EquippedSkin = "BoySkin";
        //Debug.Log($"Загружен рекорд: {_progress.HighScore} \n  ");
        //Debug.Log($"Загружен Coins: {_progress.Coins} \n  ");
        //Debug.Log($"Изменен рекорд: {_progress.HighScore}");
        //_dataManager.SavePlayerProgress(_progress);

    }

    

}
