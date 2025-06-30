using System;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using TMPro;
using UnityEngine;

public class FirebaseRemoteConfigManager : MonoBehaviour
{

    [SerializeField] private TMP_Text text ;
    private const string TEST_VARIABLE_KEY = "TestVariable";
    private const string DEFAULT_TEST_VALUE = "default_value";

    private bool _isFirebaseInitialized = false;
    private FirebaseRemoteConfig _remoteConfig;

    private void Start()
    {
        StartCoroutine(InitializeFirebase());
    }

    private IEnumerator InitializeFirebase()
    {
        // Проверка зависимостей Firebase
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        if (dependencyTask.Exception != null)
        {
            Debug.LogError($"Firebase dependency check failed: {dependencyTask.Exception}");
            yield break;
        }

        if (dependencyTask.Result == DependencyStatus.Available)
        {
            Debug.Log("Firebase dependencies resolved");
            SetupRemoteConfig();
        }
        else
        {
            Debug.LogError($"Could not resolve dependencies: {dependencyTask.Result}");
        }
    }

    private void SetupRemoteConfig()
    {
        _remoteConfig = FirebaseRemoteConfig.DefaultInstance;

        // Установка значений по умолчанию
        var defaults = new System.Collections.Generic.Dictionary<string, object>
        {
            { TEST_VARIABLE_KEY, DEFAULT_TEST_VALUE }
        };

        _remoteConfig.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("RemoteConfig defaults set");
                FetchConfigValues();
            }
            else
            {
                Debug.LogError("Default values setup failed");
            }
        });
    }

    private void FetchConfigValues()
    {
        Debug.Log("Fetching RemoteConfig values...");

        // Установка времени кэширования (0 секунд для разработки)
        TimeSpan cacheExpiration = TimeSpan.Zero;

        Task fetchTask = _remoteConfig.FetchAsync(cacheExpiration);
        fetchTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Fetch canceled");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Fetch encountered an error");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Fetch completed");
                _remoteConfig.ActivateAsync().ContinueWithOnMainThread(activateTask =>
                {
                    Debug.Log("RemoteConfig activated");
                    DisplayConfigValues();
                });
            }
        });
    }

    private void DisplayConfigValues()
    {
        string testValue = _remoteConfig.GetValue(TEST_VARIABLE_KEY).StringValue;
        Debug.Log($"___________________________________");
        Debug.Log($"RemoteConfig TestVariable = {testValue}");
        text.text = testValue;
        Debug.Log($"___________________________________");

        _isFirebaseInitialized = true;
    }

    public Color GetColorValue (string variableKey)
    {

        var hex = _remoteConfig.GetValue(variableKey).StringValue;
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
        //return int.Parse(_remoteConfig.GetValue(variableKey).StringValue, NumberStyles.HexNumber);
    }

    private void OnGUI()
    {
        if (!_isFirebaseInitialized)
        {
            GUI.Label(new Rect(10, 10, 300, 30), "Initializing Firebase...");
        }
        else
        {
            string testValue = _remoteConfig.GetValue(TEST_VARIABLE_KEY).StringValue;

            GUI.Label(new Rect(10, 10, 300, 30), $"TestVariable: {testValue}");
        }

        if (GUI.Button(new Rect(10, 50, 200, 30), "Refresh Config"))
        {
            FetchConfigValues();
        }
    }
}
