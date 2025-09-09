using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static bool AppIsQuitting { get; private set; } = false;
    private static T instance;
    [SerializeField] protected bool dontDestroyOnLoad = true;
    private static readonly object lockObj = new();
    public static T Instance
    {
        get
        {
            if (AppIsQuitting) return null;

            lock (lockObj)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject singletonObject = new(typeof(T).Name);
                        instance = singletonObject.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }


    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            Initialize();

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Initialize() { }

    protected virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;

        if (!Application.isPlaying)
            AppIsQuitting = true;
    }

    private void OnApplicationQuit()
        => AppIsQuitting = true;
}
