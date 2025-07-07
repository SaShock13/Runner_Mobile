using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesAssetProvider:IAsssetProvider
{
    public async Task<GameObject> LoadSkin(string skinId)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(skinId);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return handle.Result; 
        }
        else
        {
            Debug.LogError($"Failed to load skin: {skinId}");
            return null;
        }
    }
}
