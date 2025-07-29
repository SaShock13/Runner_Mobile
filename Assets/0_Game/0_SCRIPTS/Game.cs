using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Game : MonoBehaviour
{
    private bool isPaused = false;
	private EventBus _eventBus;
	private Player _player;
	private PlayerStats _playerStats;
	private IDataService _dataService;
    private PlayerProgress _playerProgress;
    private DistanceMeasurer _distanceMeasurer;
    private float startTime;
    private LevelGenerator _levelGenerator;
    private SoundManager _soundManager;
    private Tutorial _tutorial;
    private bool _isFirtTime = true;

    [Inject]
	public void Construct(EventBus eventBus, Player player, PlayerProgress progress, PlayerStats playerStats, IDataService dataService
        , LevelGenerator levelGenerator , Tutorial tutorial, DistanceMeasurer distanceMeasurer, SoundManager soundManager)
	{
		_eventBus = eventBus;
        _player = player;
        _playerStats = playerStats;
        _dataService = dataService;
        _playerProgress = progress;
        _levelGenerator = levelGenerator;
        _tutorial = tutorial;
        _distanceMeasurer = distanceMeasurer;
        _soundManager = soundManager;
	}

    private void Awake()
    {
        _eventBus.OnRequestPauseEvent += OnRequestPause;
        _eventBus.OnMenuRequestEvent += OnRequestMenu;
        _eventBus.OnPlayerDeathEvent += PlayerDeath;
        _eventBus.OnGameRestartRequestEvent += RestartGame;
        _eventBus.OnGameStartRequestEvent += StartGame;
        _eventBus.OnPlayerProgressResetRequestEvent += RequestDataReset;
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        _eventBus.OnRequestPauseEvent -= OnRequestPause;
        _eventBus.OnMenuRequestEvent -= OnRequestMenu;
        _eventBus.OnPlayerDeathEvent -= PlayerDeath;
        _eventBus.OnGameRestartRequestEvent -= RestartGame;
        _eventBus.OnGameStartRequestEvent -= StartGame;
        _eventBus.OnPlayerProgressResetRequestEvent -= RequestDataReset;
    }
    private void OnRequestMenu()
    {
        OnRequestPause();
        _soundManager.PlayMusic(Sounds.menuMusic);
        _eventBus.PublishOnMenuEvent();
    }

    private void RequestDataReset()
    {
        _isFirtTime = true;
        _dataService.DeleteAllData();
        _eventBus.PublishOnPlayerProgressResetEvent();
    }

    private void StartGame()
    {
        _player.ResetPlayer();
        _soundManager.PlayMusic(Sounds.gameMusic);
        _levelGenerator.ResetLevel();
        _eventBus.PublishOnGameResetEvent();
        if (_playerProgress.IsFirstTime)
        {
            _tutorial.SetInput(_player._input);
            _tutorial.StartTutorial();
            _playerProgress.IsFirstTime = false;
            _dataService.SavePlayerProgress(_playerProgress);
        }
        else _eventBus.PublishOnTutorialFinishedEvent();      
        _eventBus.PublishOnGameStartEvent();
        _levelGenerator.StartGeneration();
        _distanceMeasurer.StartMeasure();
        isPaused = false;
        Time.timeScale = 1;
        startTime = Time.time;

    }

    private void OnRequestPause()
    {
        DebugUtils.LogEditor($"GameManager Get pause Request {this}");
        if (isPaused) Resume();
        else Pause();
    }

    private void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
        _eventBus.PublishOnPauseEvent();
    }
    
    private void Resume()
    {
        Time.timeScale = 1;
        _soundManager.PlayMusic(Sounds.gameMusic);
        isPaused = false;
        _eventBus.PublishOnResumeEvent();
    }

    private void PlayerDeath()
    {
        _soundManager.PlayMusic(Sounds.menuMusic);
        _player.Death();
        Firebase.Analytics.FirebaseAnalytics
                                            .LogEvent("Player_Death", "percent", 0.5f);
        Firebase.Analytics.FirebaseAnalytics
                                            .LogEvent("RaceDuration", "duration", Time.time - startTime);
        if (_playerProgress.HighScore < _playerStats.bonuses)
        {
            _playerProgress.HighScore = _playerStats.bonuses;
        }
        _playerProgress.Coins += _playerStats.coins;
        _playerProgress.Diamonds += _playerStats.diamonds;
        _dataService.SavePlayerProgress(_playerProgress);
        _distanceMeasurer.StopMeasure();

        DebugUtils.LogEditor($" всего.Coins  {_playerProgress.Coins}");
        DebugUtils.LogEditor($" всего.Diamonds {_playerProgress.Diamonds}");
        DebugUtils.LogEditor($" всего.HighScores {_playerProgress.HighScore}");
        Invoke("GameOver", 2);
    }

    private void GameOver()
    {

        DebugUtils.LogEditor($"GameOver {this}");
        _eventBus.PublishOnGameOverEvent();

    }

    private void RestartGame()
    {
        isPaused = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
