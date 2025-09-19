using UnityEngine;
using UnityEngine.SceneManagement;
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
            if (instance == null)
                instance = FindObjectOfType<T>(true); // 씬/프리팹에 있는 것만 찾기
            return instance; // 없으면 null 반환
        }
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this as T;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        Initialize();
    }

    protected virtual void Initialize() { }

    protected virtual void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    private void OnApplicationQuit()
    {
        AppIsQuitting = true;
    }
}
