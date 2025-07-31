using UnityEngine;

public class ScreenShooter : MonoBehaviour
{
    private int num = 0;
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.U))
        {
            ScreenCapture.CaptureScreenshot($"ScreenShot{num++}.jpg");
        }

    }
}
