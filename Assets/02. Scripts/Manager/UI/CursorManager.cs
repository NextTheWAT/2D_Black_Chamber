using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    private Dictionary<UIKey, CrosshairCursor> crosshairDict = new();

    public CrosshairCursor NowCrosshair
    {
        get
        {
            if (crosshairDict.TryGetValue(currentUIKey, out CrosshairCursor value))
                return value;
            return null;
        }
        private set
        {
            crosshairDict[currentUIKey] = value;
        }
    }


    [Header("UIKey")]
    [SerializeField] private List<UIKey> gameplayKeys = new() { UIKey.Game, UIKey.Lobby };
    private UIKey currentUIKey;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void OnEnable()
    {
        if (GameManager.AppIsQuitting) return;
        UIManager.Instance.OnUIActiveChanged += OnUIActiveChanged;
    }

    private void OnDisable()
    {
        if (GameManager.AppIsQuitting) return;
        UIManager.Instance.OnUIActiveChanged -= OnUIActiveChanged;
    }

    public void AddCrosshair(CrosshairCursor crosshair, UIKey uiKey)
    {
        crosshairDict[uiKey] = crosshair;
    }

    private void OnUIActiveChanged(UIBase uIBase, bool active)
    {
        if (active)
        {
            SetGameplayCursor(false);
        }
        else
        {
            if (currentUIKey == UIKey.Title)
                SetGameplayCursor(false);
            else
                SetGameplayCursor(UIActiveCounter.ActiveUICount <= 0);
        }
    }



    public void ApplyByUIKey(UIKey key) //UIKey에 맞춰 커서 상태 적용
    {
        currentUIKey = key;
        bool gameplay = gameplayKeys.Contains(key);
        SetGameplayCursor(gameplay);
    }

    public void SetGameplayCursor(bool on)
    {
        if (on)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            if (NowCrosshair) NowCrosshair.gameObject.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (NowCrosshair) NowCrosshair.gameObject.SetActive(false);
        }
    }
}

