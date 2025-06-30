using UnityEngine;

public interface IDataStorage 
{
    void Save(string key, string data);
    string Load(string key);
    bool HasKey(string key);
    void Delete(string key);
}
