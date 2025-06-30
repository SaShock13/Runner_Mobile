using System.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

public class DataProvider : IDataService
{

    private const string PROGRESS_KEY = "PlayerProgress";
    private PlayerProgress _cachedProgress;

    [Inject] private IDataStorage _storage;
    [Inject] private EventBus _eventBus;

    public PlayerProgress LoadPlayerProgress()
    {
        // Используем кеш, если данные уже загружены
        if (_cachedProgress != null) return _cachedProgress;

        if (_storage.HasKey(PROGRESS_KEY))
        {
            try
            {
                string json = _storage.Load(PROGRESS_KEY);
                _cachedProgress = JsonUtility.FromJson<PlayerProgress>(json);
                //_eventBus.PublishOProgressLoadedEvent();
                return _cachedProgress;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load progress: {ex.Message}");
            }
        }

        // Создаем новый прогресс, если сохранений нет
        _cachedProgress = new PlayerProgress();
        //_eventBus.Publish(new NewProgressCreatedEvent(_cachedProgress));
        return _cachedProgress;
    }



    public void SavePlayerProgress(PlayerProgress progress)
    {
        try
        {
            _cachedProgress = progress;
            _cachedProgress.LastSaveTime = DateTime.UtcNow;

            string json = JsonUtility.ToJson(progress);
            _storage.Save(PROGRESS_KEY, json);


            Debug.Log($"SavePlayerProgress sucseed {this}");
            //_eventBus.Publish(new ProgressSavedEvent(progress));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Save failed: {ex.Message}");
        }
    }

    public bool HasSavedData() => _storage.HasKey(PROGRESS_KEY);

    public void DeleteAllData()
    {
        _storage.Delete(PROGRESS_KEY);
        _cachedProgress = null;
    }

}
