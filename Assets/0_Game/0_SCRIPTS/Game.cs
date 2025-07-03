using System;
using ModestTree;
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
    private float startTime;
    private LevelGenerator _levelGenerator;
    private Tutorial _tutorial;
    private bool _isFirtTime = true;

    [Inject]
	public void Construct(EventBus eventBus, Player player, PlayerProgress progress, PlayerStats playerStats, IDataService dataService
        , LevelGenerator levelGenerator , Tutorial tutorial)
	{
		_eventBus = eventBus;
        _player = player;
        _playerStats = playerStats;
        _dataService = dataService;
        _playerProgress = progress;
        _levelGenerator = levelGenerator;
        _tutorial = tutorial;
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

    private void OnRequestMenu()
    {
        OnRequestPause();
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
        Time.timeScale = 1;
        startTime = Time.time;
    }

    private void OnRequestPause()
    {
        Debug.Log($"GameManager Get pause Request {this}");
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
        isPaused = false;
        _eventBus.PublishOnResumeEvent();
    }

    private void PlayerDeath()
    {
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

        Debug.Log($" всего.Coins  {_playerProgress.Coins}");
        Debug.Log($" всего.Diamonds {_playerProgress.Diamonds}");
        Debug.Log($" всего.HighScores {_playerProgress.HighScore}");
        Invoke("GameOver", 2);
    }

    private void GameOver()
    {

        Debug.Log($"GameOver {this}");
        _eventBus.PublishOnGameOverEvent();

    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
