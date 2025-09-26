using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePopup : UIBase
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button dimmerButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button lobbyButton;   // 로비로
    [SerializeField] private Button quitButton;    // 게임 종료

    [Header("Anim")]
    [SerializeField] private float fadeDuration = 0.18f;

    [SerializeField] private string lobbySceneName = "LobbyScene";

    private Coroutine fadeCo;
    private float prevTimeScale = 1f; // 닫을 때 복원용
    private bool suppressRestoreOnce = false;

    private bool cursorPrevVisible;
    private CursorLockMode cursorPrevLock;

    private void Reset()
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        if (!dimmerButton) dimmerButton = transform.Find("Dimmer")?.GetComponent<Button>();
        if (!continueButton) continueButton = transform.Find("Window/ButtonGroup/ContinueButton")?.GetComponent<Button>();
        if (!settingButton) settingButton = transform.Find("Window/ButtonGroup/SettingButton")?.GetComponent<Button>();
        if (!lobbyButton) lobbyButton = transform.Find("Window/ButtonGroup/LobbyButton")?.GetComponent<Button>();
        if (!quitButton) quitButton = transform.Find("Window/ButtonGroup/QuitButton")?.GetComponent<Button>();
    }

    private void Start()
    {
        //gameObject.SetActive(false); // 기본 비활성   이녀석이 문제였어
    }

    protected override void OnOpen()
    {
        CursorManager.Instance?.SetGameplayCursor(false);

        if (!Initialized)
        {
            if (dimmerButton) dimmerButton.onClick.AddListener(RequestClose);
            if (continueButton) continueButton.onClick.AddListener(RequestClose);
            if (settingButton) settingButton.onClick.AddListener(() => UIManager.Instance.OpenUI<SettingPopup>());
            if (lobbyButton) lobbyButton.onClick.AddListener(OnClickLobby);
            if (quitButton) quitButton.onClick.AddListener(OnClickQuit);
            Initialized = true;
        }

        // 시간 멈춤 + 커서 표시
        if (Time.timeScale > 0f)
            prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        cursorPrevVisible = Cursor.visible;
        cursorPrevLock = Cursor.lockState;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            if (fadeCo != null) StopCoroutine(fadeCo);
            fadeCo = StartCoroutine(Fade(0f, 1f, fadeDuration));
        }

        UpdateLobbyButtonVisibility();
    }

    public void RequestClose()
    {
        if (!gameObject.activeInHierarchy) return;
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(CloseSequence());
    }

    private IEnumerator CloseSequence()
    {
        if (canvasGroup)
        {
            canvasGroup.interactable = false;
            yield return Fade(1f, 0f, fadeDuration);
            canvasGroup.blocksRaycasts = false;
        }
        UIManager.Instance.CloseUI<PausePopup>();
    }

    protected override void OnClose()
    {
        CursorManager.Instance?.SetGameplayCursor(true);

        // 설정으로 배턴 넘기는 상황이면 이번 한 번은 복원 생략 (일시정지 유지)
        if (!suppressRestoreOnce)
            Time.timeScale = prevTimeScale;
        else
            suppressRestoreOnce = false; // 1회성 플래그 초기화

        Cursor.visible = cursorPrevVisible;
        Cursor.lockState = cursorPrevLock;

        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator Fade(float from, float to, float dur)
    {
        float t = 0f;
        if (dur <= 0f)
        {
            if (canvasGroup) canvasGroup.alpha = to;
            yield break;
        }
        if (canvasGroup) canvasGroup.alpha = from;

        // unscaledDeltaTime 사용
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            if (canvasGroup) canvasGroup.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        if (canvasGroup) canvasGroup.alpha = to;
    }

    private void OpenSettingFromPause()
    {
        // 1) 설정 먼저 열기
        var setting = UIManager.Instance.OpenUI<SettingPopup>();

        // 2) 설정이 닫히면 Pause 재오픈 (코루틴으로 감지)
        if (setting != null)
            StartCoroutine(ReopenWhenClosed(setting));

        // 3) 이번 닫힘은 복원하지 말고(일시정지 유지), Pause 닫기
        suppressRestoreOnce = true;
        RequestClose();
    }

    private IEnumerator ReopenWhenClosed(SettingPopup setting)
    {
        // SettingPopup이 비활성화될 때까지 대기
        while (setting != null && setting.gameObject.activeInHierarchy)
            yield return null;

        // 설정 닫힘 후 Pause 자동 복귀
        UIManager.Instance.OpenUI<PausePopup>();
    }

    private void OnClickLobby()
    {
        StartCoroutine(ReturnToLobby());
    }

    private IEnumerator ReturnToLobby()
    {
        RequestClose();
        while (gameObject.activeInHierarchy) yield return null; // 닫힐 때까지 대기
        if (Time.timeScale == 0f) Time.timeScale = 1f;
        if (!Application.CanStreamedLevelBeLoaded(lobbySceneName))
        {
            Debug.LogError($"씬 '{lobbySceneName}' 없음");
            yield break;
        }
        SceneManager.LoadScene(lobbySceneName);
    }

    private void UpdateLobbyButtonVisibility()
    {
        if (!lobbyButton) return;

        bool isLobby = SceneManager.GetActiveScene().name == lobbySceneName;
        lobbyButton.gameObject.SetActive(!isLobby);   // 로비면 숨김, 그 외엔 표시
                                                      // 만약 레이아웃 유지하고 싶으면 ↓로 교체
                                                      // lobbyButton.interactable = !isLobby;
    }

    private void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
