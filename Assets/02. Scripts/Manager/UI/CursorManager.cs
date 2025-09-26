using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("GameCanvas/Crosshair")]
    [SerializeField] private GameObject crosshairUI;

    [Header("UIKey")]
    [SerializeField] private List<UIKey> gameplayKeys = new() { UIKey.Game };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyByUIKey(UIKey key) //UIKey에 맞춰 커서 상태 적용
    {
        bool gameplay = gameplayKeys.Contains(key);
        SetGameplayCursor(gameplay);
    }

    public void SetGameplayCursor(bool on)
    {
        if (on)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            if (crosshairUI) crosshairUI.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (crosshairUI) crosshairUI.SetActive(false);
        }
    }
}

