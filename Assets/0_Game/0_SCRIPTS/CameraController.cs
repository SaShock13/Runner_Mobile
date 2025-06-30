using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera backCamera;
    [SerializeField] private CinemachineCamera frontCamera;
    private Player _player;


    [Inject]
    public void Construct(Player player)
    {
        _player = player;
    }


    private void Awake()
    {
        
        backCamera.Follow = _player.transform;
        backCamera.LookAt = _player.transform;
        frontCamera.Follow = _player.transform;
        frontCamera.LookAt = _player.transform;
        frontCamera.Priority = 0;
        backCamera.Priority = 10;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            frontCamera.Priority = 0;
            backCamera.Priority = 10;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            frontCamera.Priority = 10;
            backCamera.Priority = 0;
        }

    }
}
