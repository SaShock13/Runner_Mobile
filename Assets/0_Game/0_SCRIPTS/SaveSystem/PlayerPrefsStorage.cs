using UnityEngine;

public class PlayerPrefsStorage : IDataStorage
{
    public void Delete(string key)
    {
        if (HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
        }
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public string Load(string key)
    {
       return PlayerPrefs.GetString(key);
    }

    public void Save(string key, string data)
    {
        PlayerPrefs.SetString(key, data);
    }
}
