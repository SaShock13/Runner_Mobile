using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class SkinChanger : MonoBehaviour
{    
    private GameObject girlPrefab;
    private GameObject boyPrefab;
    private Player _player;
    private IAsssetProvider _asssetProvider;
    [Inject] private PlayerProgress _progress;
    [Inject] private EventBus _eventBus;



    [Inject]
    public void Construct(IAsssetProvider asssetProvider)
    {
        _asssetProvider = asssetProvider;
    }

    private void OnEnable()
    {
        _eventBus.OnProgressLoadedEvent += ProgressLoaded;
    }

    private void ProgressLoaded()
    {
        LoadSkin();
    }

    private async void LoadSkin()
    {
        var currentPrefab = await _asssetProvider.LoadSkin(_progress.EquippedSkin);
        _player.SetSkinPrefab(currentPrefab);
    }

    private void Awake()
    {
        _player = GetComponent<Player>();       
    }

    private async void Start()
    {
        girlPrefab = await _asssetProvider.LoadSkin("GirlSkin");
        boyPrefab = await _asssetProvider.LoadSkin("BoySkin");
    }

    

    public void SetGirlSkin()
    {
        _player.SetSkinPrefab(girlPrefab);
    }
    public void SetBoySkin()
    {
        _player.SetSkinPrefab(boyPrefab);
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.U))
        {

            Debug.Log($"Выгрузка {Addressables.ReleaseInstance(girlPrefab)}"); 

        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log($"Выгрузка {Addressables.ReleaseInstance(boyPrefab)}");
        }


    }
}
