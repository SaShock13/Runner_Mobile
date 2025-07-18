using UnityEngine;

public class VSyncTest : MonoBehaviour
{
    void Start()
    {
        int vsync = QualitySettings.vSyncCount;
        Application.targetFrameRate = 72;
        DebugUtils.LogEditor($"vSyncCount = {vsync} ({(vsync > 0 ? "включён" : "выключен")})");
    }

   
}
