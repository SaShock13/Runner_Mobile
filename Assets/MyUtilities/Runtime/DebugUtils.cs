using UnityEngine;
using System.Diagnostics;

public static class DebugUtils 
{
    [Conditional("UNITY_EDITOR")]
    public static void LogEditor(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
    public static void LogDevBuild(string message)
    {
        UnityEngine.Debug.Log(message);
    }
    [Conditional("UNITY_EDITOR")]
    public static void LogWarningEditor(string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogErrorEditor(string message)
    {
        UnityEngine.Debug.LogError(message);
    }

    // Расширение по нужде — например, лог в конкретный цвет, файл, UI и т.д.
}
