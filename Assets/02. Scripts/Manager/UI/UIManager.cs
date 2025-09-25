using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private readonly Dictionary<string, UIBase> _cache = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private T FindOrCache<T>() where T : UIBase
    {
        string key = typeof(T).Name;
        if (_cache.TryGetValue(key, out var ui) && ui != null)
            return ui as T;

        var found = FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (found != null)
        {
            _cache[key] = found;
            return found;
        }
        return null;
    }

    public T OpenUI<T>() where T : UIBase 
    {
        var ui = FindOrCache<T>();
        if (ui == null)
        {
            Debug.LogError($"[UIManager] {typeof(T).Name}를 씬에서 찾을 수 없음");
            return null;
        }
        ui.OpenUI();
        return ui;
    }

    public void CloseUI<T>() where T : UIBase
    {
        var ui = FindOrCache<T>();
        if (ui == null) return;
        ui.CloseUI();
    }
}
