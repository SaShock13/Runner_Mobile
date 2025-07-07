using System.Threading.Tasks;
using UnityEngine;

public interface IAsssetProvider
{
    Task<GameObject> LoadSkin(string skinId);
}
