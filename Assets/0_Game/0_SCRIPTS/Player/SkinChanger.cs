using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class SkinChanger : MonoBehaviour
{    
    private GameObject girlPrefab;
    private GameObject boyPrefab;
    private Player _player;
    private IAsssetProvider _asssetProvider;
    private PlayerProgress _progress;
    private EventBus _eventBus;

    [Inject]
    public void Construct(IAsssetProvider asssetProvider, PlayerProgress progress, EventBus eventBus)
    {
        _asssetProvider = asssetProvider;
        _progress = progress;
        _eventBus = eventBus;
    }
    private void OnEnable()
    {
        _eventBus.OnProgressLoadedEvent += ProgressLoaded;
    }
    private void OnDisable()
    {
        _eventBus.OnProgressLoadedEvent -= ProgressLoaded;        
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

            DebugUtils.LogEditor($"Выгрузка {Addressables.ReleaseInstance(girlPrefab)}"); 

        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            DebugUtils.LogEditor($"Выгрузка {Addressables.ReleaseInstance(boyPrefab)}");
        }
    }
}
