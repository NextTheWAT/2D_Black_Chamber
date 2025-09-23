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

    public void ShowOnly(UIKey key) //�´� ĵ������ Ȱ��ȭ, ������ ��Ȱ��ȭ
    {
        // ���� ĵ������ ���
        foreach (var pair in _map)
            pair.Value.gameObject.SetActive(pair.Key == key);

        // �׻� �ѵ� ĵ������ ����
        foreach (var c in alwaysOnCanvases)
            if (c) c.gameObject.SetActive(true);
    }
}
