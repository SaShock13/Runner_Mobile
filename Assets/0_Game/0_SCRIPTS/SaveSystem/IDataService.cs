public interface IDataService 
{
    PlayerProgress LoadPlayerProgress();
    void SavePlayerProgress(PlayerProgress progress);
    void DeleteAllData();
    bool HasSavedData();
}
