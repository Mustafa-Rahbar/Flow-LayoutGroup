using UnityEngine;

public static class UnityDebugController
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void DisableLoggerOutsideOfEditor()
    {
        Debug.unityLogger.logEnabled = Debug.isDebugBuild || Application.isEditor;
    }
}