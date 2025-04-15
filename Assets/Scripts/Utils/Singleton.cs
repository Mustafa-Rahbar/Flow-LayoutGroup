using UnityEngine;

[DefaultExecutionOrder(-5)]
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance is null)
            {
                Debug.LogErrorFormat("Singleton of type: {0} not found on scene", typeof(T).Name);
            }
            return _instance;
        }
    }

    protected bool InitializeSingleton(bool persistent = true)
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarningFormat("Another instance of Singleton<{0}> detected on GO {1} destroyed", typeof(T).Name, name);
            Destroy(this);
            return false;
        }
        else
        {
            _instance = this as T;
            if (persistent) DontDestroyOnLoad(this);
            return true;
        }
    }
}