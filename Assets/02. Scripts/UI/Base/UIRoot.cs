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

    // 씬 내 UIRoot가 관리하는 캔버스들을 전부 끈다.
    public void HideAllSceneUIs(bool includeAlwaysOn = false)
    {
        foreach (var pair in _map)
            if (pair.Value) pair.Value.gameObject.SetActive(false);

        if (includeAlwaysOn)
            foreach (var c in alwaysOnCanvases)
                if (c) c.gameObject.SetActive(false);
    }

    // 메인 하나만 켜되, 추가로 몇 개 더 켜고 싶을 때(오버레이 등)
    public void ShowOnlyWithAlsoOn(UIKey key, params UIKey[] alsoOn)
    {
        ShowOnly(key); // 기존 함수 재사용  :contentReference[oaicite:0]{index=0}
        foreach (var k in alsoOn)
        {
            if (_map.TryGetValue(k, out var cv) && cv)
                cv.gameObject.SetActive(true);
        }
    }
}
