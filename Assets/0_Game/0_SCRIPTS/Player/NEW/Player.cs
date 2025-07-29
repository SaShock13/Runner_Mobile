using System.Collections;
using UnityEngine;
using Zenject;


// todo Подогнать уменьшение коллайдеров прыжка и слайда под анимации

public class Player : MonoBehaviour
{
    private GameObject currentSkin;
    [SerializeField] private Transform skinHolder;
    private PlayerHealth health;
    private PlayerMovement movement;
    public IInput _input;
    public Animator _animator;
    public PlayerEffects _playerFX;
    public bool isMobile = true;
    private PlayerAnimatorManager _animatorManager;
    private EventBus _eventBus;
    IAsssetProvider _asssetProvider;
    private Tutorial _tutorial;
    private PlayerStats _stats;
    [SerializeField] bool isInvincible = true;

    [Inject]
    public void Construct( EventBus eventBus, PlayerAnimatorManager animatorManager, IAsssetProvider asssetProvider, Tutorial tutorial, PlayerStats stats)
    {
        _eventBus = eventBus;
        _animatorManager = animatorManager;
        _asssetProvider = asssetProvider;
        _tutorial = tutorial;
        _stats = stats;
    }

    public void ResetPlayer()
    {
        var newPOs = transform.position;
        newPOs.z = -7.93f;
        transform.position = newPOs;
        _stats.ResetStats();
        if (isInvincible) { _stats.IsInvincible = true; }

    }

    private void Awake()
    {        
        movement = GetComponent<PlayerMovement>();
        _playerFX = GetComponent<PlayerEffects>();
        _input = new MobileInput(); 
        #if UNITY_EDITOR
        _input = GetComponent<KeyboardInput>();
        #endif
        movement.SetInput(_input);
    }

    private IEnumerator Start()
    {
        yield return null;
    }

    /// <summary>
    /// Когда уже загружен скин из рессурсов
    /// </summary>
    public void OnSkinLoaded()
    {
        _animatorManager.Initialize(_animator,_eventBus);
        _playerFX.Initialize();
    }

    public void SetSkinPrefab(GameObject skinPrefab)
    {
        DebugUtils.LogEditor($"SetSkinPrefab PLayer {this}");
        if (skinPrefab == null) return;
        if(currentSkin != null) Destroy(currentSkin);
        var skinGO = Instantiate(skinPrefab, skinHolder);
        currentSkin = skinGO;
        _animator = skinGO.GetComponent<Animator>();
        OnSkinLoaded();
    }

    public void Death()
    {
        movement.Stop();
    }
}
