using System.Collections.Generic;
using UnityEngine;

public class UIRoot : MonoBehaviour
{
    [System.Serializable]
    public class CanvasEntry
    {
        public UIKey key;
        public Canvas canvas;
    }

    [SerializeField] private List<CanvasEntry> canvases = new();

    [SerializeField] private List<Canvas> alwaysOnCanvases = new();

    private Dictionary<UIKey, Canvas> _map;

    private void Awake()
    {
        _map = new Dictionary<UIKey, Canvas>();
        foreach (var entry in canvases)
        {
            if (entry.canvas != null)
                _map[entry.key] = entry.canvas;
        }
    }

    public void ShowOnly(UIKey key) //맞는 캔버스만 활성화, 나머지 비활성화
    {
        // 씬별 캔버스만 토글
        foreach (var pair in _map)
            pair.Value.gameObject.SetActive(pair.Key == key);

        // 항상 켜둘 캔버스는 유지
        foreach (var c in alwaysOnCanvases)
            if (c) c.gameObject.SetActive(true);
    }
}
