using System;
using UnityEngine;
using Zenject;

public class DataProvider : IDataService
{
    private const string PROGRESS_KEY = "PlayerProgress";
    private PlayerProgress _cachedProgress;
    private IDataStorage _storage;

    [Inject]
    public DataProvider(IDataStorage storage)
    {
        _storage = storage;
    }
    public PlayerProgress LoadPlayerProgress()
    {
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
        _cachedProgress = new PlayerProgress();
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
