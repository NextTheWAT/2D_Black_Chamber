using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public Action<UIBase, bool> OnUIActiveChanged;
    private readonly Dictionary<string, UIBase> _cache = new();

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
            Debug.LogError($"[UIManager] {typeof(T).Name}�� ������ ã�� �� ����");
            return null;
        }
        ui.OpenUI();
        OnUIActiveChanged?.Invoke(ui, true);
        return ui;
    }

    public void CloseUI<T>() where T : UIBase
    {
        var ui = FindOrCache<T>();
        if (ui == null) return;
        ui.CloseUI();
        OnUIActiveChanged?.Invoke(ui, false);
    }
}
