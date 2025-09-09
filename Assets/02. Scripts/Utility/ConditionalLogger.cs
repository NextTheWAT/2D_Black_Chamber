using UnityEngine;

public static class ConditionalLogger
{
    // https://www.youtube.com/watch?v=2NbVjxKfMWI&t=4s

    [System.Diagnostics.Conditional("DEBUG_LOG")]
    public static void Log(object message)
        => Debug.Log(message);

    [System.Diagnostics.Conditional("DEBUG_LOG")]
    public static void LogWarning(object message)
        => Debug.LogWarning(message);

    [System.Diagnostics.Conditional("DEBUG_LOG")]
    public static void LogError(object message)
        => Debug.LogError(message);
}
