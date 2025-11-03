using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActiveCounter : MonoBehaviour
{
    public static int ActiveUICount = 0;
    private UIBase uiBase;

    private void Awake()
    {
        uiBase = GetComponent<UIBase>();
    }

    private void OnEnable()
    {
        if (GameManager.AppIsQuitting) return;
        ActiveUICount++;
        UIManager.Instance.OnUIActiveChanged?.Invoke(uiBase, true);
    }

    private void OnDisable()
    {
        if (GameManager.AppIsQuitting) return;
        ActiveUICount--;
        UIManager.Instance.OnUIActiveChanged?.Invoke(uiBase, false);
    }
}
