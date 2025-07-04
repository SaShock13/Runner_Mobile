using Cysharp.Threading.Tasks;
using ModestTree;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static UnityEngine.Rendering.VolumeComponent;

public class MenuView : MonoBehaviour
{
    private Player _player;
    private EventBus _eventBus;
    PlayerMovement playerMovement;
    private PlayerHealth health;

    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button menuBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button shopBtn;
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject gameMenuPanel;
    [SerializeField] TMP_Text livesText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text speedText;
    [SerializeField] TMP_Text infoText;
    [SerializeField] TMP_Text fpsText;
    [SerializeField] TMP_Text bonusCounterText;
    [SerializeField] TMP_Text coinsCounterText;
    [SerializeField] TMP_Text diamondsCounterText;
    [SerializeField] TMP_Text speedBoostText;
    [SerializeField] TMP_Text invincibilityText;
    [SerializeField] TMP_Text x2Text;
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] GameObject playerNameInputGO;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] GameObject playerNameTextGO;
    [SerializeField] GameObject weatherVidget;
    [SerializeField] WeatherApiDialog weather;
    [SerializeField] bool showWeather = false;

    private CompositeDisposable _disposable = new();
    private SkinChanger _skinChanger;
    private PlayerProgress _progress;
    private IDataService _dataService;
    private bool isWeatherLoaded = false;
    private TextMeshProUGUI pauseButtonText;
    private bool isFirstEnter = true;

    [Inject]
    public void Construct(Player player, EventBus eventBus, PlayerProgress progress, IDataService dataService)
    {
        _progress = progress;
        _eventBus = eventBus;
        _player = player;
        _dataService = dataService;
        _skinChanger = player.GetComponent<SkinChanger>();
    }


    private void Awake()
    {
        _eventBus.OnPauseEvent += PauseGame;
        _eventBus.OnMenuEvent += ShowMenu;
        _eventBus.OnResumeEvent += ResumeGame;
        _eventBus.OnPlayerDamagedEvent += PlayerDamaged;
        _eventBus.OnGameOverEvent += GameOver;
        _eventBus.OnGameStartEvent += StartGame;
        _eventBus.OnBonusAmountChangedEvent += UpdateBonusAmount;
        _eventBus.OnSpeedChangedEvent += UpdateSpeed;
        _eventBus.OnCoinsAmountChangedEvent += UpdateCoins;
        _eventBus.OnDiamondAmountChangedEvent += UpdateDiamonds;
        _eventBus.OnProgressLoadedEvent += OnProgressLoaded;
        _eventBus.OnSpeedBoostCollectedEvent += SpeedBoost;
        _eventBus.OnInvincibilityCollectedEvent += Invincibility;
        _eventBus.OnMultiplyerX2CollectedEvent += MultiplyerX2;
        shopPanel.SetActive(false);
        gameMenuPanel.SetActive(false);
        pauseBtn.onClick.AddListener(OnPauseClicked);
        menuBtn.onClick.AddListener(OnMenuClicked);
        restartBtn.onClick.AddListener(OnRestartClicked);
        resumeBtn.onClick.AddListener(OnResumeClicked);
        shopBtn.onClick.AddListener(OnShopClicked);
        mainMenuBtn.onClick.AddListener(OnMainMenuClicked);
        pauseButtonText = pauseBtn.GetComponentInChildren<TextMeshProUGUI>();
        x2Text.enabled = false;
        invincibilityText.enabled = false;
        speedBoostText.enabled = false;
        if (showWeather)
        {
            weather.enabled = true;
            weatherVidget.SetActive(true);
        }
        else
        {
            weather.enabled = false;
            weatherVidget.SetActive(false);
        }
    }

    private void ShowMenu()
    {
        gameMenuPanel.SetActive(true);
        //ShopPanel.SetActive(true);
    }

    private void ShowShop()
    {
        shopPanel.SetActive(true);
    }



    private async void MultiplyerX2(float duration)
    {
        x2Text.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        x2Text.enabled = false;

    }

    private async void Invincibility(float duration)
    {
        invincibilityText.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        invincibilityText.enabled = false;
    }

    private async void SpeedBoost(float duration)
    {
        speedBoostText.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        speedBoostText.enabled = false;
    }

    private void OnProgressLoaded()
    {

        Debug.Log($"OnProgressLoaded  in hud  name {_progress.Name}");
        if (!_progress.Name.IsEmpty())
        {
            isFirstEnter = false;

            playerNameText.text = _progress.Name;
            playerNameTextGO.SetActive(true);
            playerNameInputGO.SetActive(false);

        }
        else
        { 
            playerNameInputGO.SetActive(true);
            playerNameTextGO.SetActive(false);
        }
    }

    private void UpdateDiamonds(int diamonds)
    {
        diamondsCounterText.text = diamonds.ToString();
    }

    private void UpdateCoins(int coins)
    {
        coinsCounterText.text = coins.ToString();
    }

    private void OnRestartClicked()
    {
        _eventBus.PublishOnGameRestartRequestEvent();
    }

    public void OnClearProgressBtnClicked()
    {
        _eventBus.PublishOnPlayerProgressResetRequestEvent();
        
    }



    public void OnPlayButtonClicked()
    {
        if (isFirstEnter)
        {
            string playerName;
            if (playerNameInput.text.Length > 0)
            {
                playerName = playerNameInput.text;

            }
            else playerName = "Инкогнито";

            _progress.Name = playerName;
            _dataService.SavePlayerProgress(_progress); 
        }

        Debug.Log($"on start _progress.Name  {_progress.Name}");
        _eventBus.PublishOnGameStartRequestEvent();
    }

    public void OnCloseMenuClck()
    {
        _eventBus.PublishOnRequestPauseEvent();
        gameMenuPanel.SetActive(false);
    }

    private void StartGame()
    {
        startMenuPanel.SetActive(false);
        nameText.text = _progress.Name;
    }

    private void GameOver()
    {
        deathPanel.SetActive(true);
    }

    private void UpdateSpeed(float speed)
    {
        speedText.text = speed.ToString("F");
    }

    private void UpdateBonusAmount(int amount)
    {
        bonusCounterText.text = amount.ToString();
    }

    private void PlayerDamaged(int max, int current)
    {
        livesText.text = current.ToString();
    }

    private void OnPauseClicked()
    {
        _eventBus.PublishOnRequestPauseEvent();
    }
    
    private void OnMenuClicked()
    {
        _eventBus.PublishOnMenuRequestEvent();
    }

    private void OnResumeClicked()
    {
        OnCloseMenuClck();
    }

    private void OnShopClicked()
    {
        //gameMenuPanel.SetActive(false);
        shopPanel.SetActive(true);
    }
    private void OnMainMenuClicked()
    {
        gameMenuPanel.SetActive(false );
        startMenuPanel.SetActive(true );
    }

    private void ResumeGame( )
    {
        pauseButtonText.text = "ПАУЗА";
    }

    private void PauseGame()
    {
        pauseButtonText.text = "Возобновить";
    }


    public void SetBoySkin() // todo перенести  в магазин
    {
        if (_progress.IsEnoughCoins(10))
        {
            _progress.SpendCoin(10);
            _dataService.SavePlayerProgress(_progress);
            _skinChanger.SetBoySkin();
        }
    }
     public void SetGirlSkin()
    {
        if(_progress.IsEnoughCoins(15))
        {
            _progress.SpendCoin(15);
            _dataService.SavePlayerProgress(_progress);
            _skinChanger.SetGirlSkin();
        }
        
    }

    IEnumerator Start()
    {
        playerMovement = _player.gameObject.GetComponent<PlayerMovement>();
        health = _player.gameObject.GetComponent<PlayerHealth>();
        //playerNameText.gameObject.SetActive(false);
        //playerNameInput.gameObject.SetActive(false);
        startMenuPanel.SetActive(true);
        
        while (true)
        {
            fpsText.text = (1 / Time.deltaTime).ToString("F1");
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowWeather();
        }

    }

    public void ShowWeather()
    {
        if (!isWeatherLoaded)
        {
            weather.enabled = true; 
            isWeatherLoaded = true;
        }
        weatherVidget.SetActive(!weatherVidget.activeInHierarchy);        
    }


    private void OnDestroy()
    {
        _disposable?.Dispose();
    }
}
