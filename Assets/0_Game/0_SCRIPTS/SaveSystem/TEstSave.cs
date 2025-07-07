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
        _eventBus.PublishOProgressLoadedEvent();
    }
}
