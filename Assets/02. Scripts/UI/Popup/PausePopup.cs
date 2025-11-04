using Esper.Freeloader;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class PausePopup : UIBase
{
    [Header("Refs")]
    [SerializeField] private Button dimmerButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button lobbyButton;    // 로비로
    [SerializeField] private Button quitButton;    // 게임 종료

    [SerializeField] private string lobbySceneName = "LobbyScene";

    private float prevTimeScale = 1f; // 닫을 때 복원용
    private bool suppressRestoreOnce = false;

    private bool cursorPrevVisible;
    private CursorLockMode cursorPrevLock;

    private void Reset()
    {
        if (!dimmerButton) dimmerButton = transform.Find("Dimmer")?.GetComponent<Button>();
        if (!continueButton) continueButton = transform.Find("Window/ButtonGroup/ContinueButton")?.GetComponent<Button>();
        if (!settingButton) settingButton = transform.Find("Window/ButtonGroup/SettingButton")?.GetComponent<Button>();
        if (!lobbyButton) lobbyButton = transform.Find("Window/ButtonGroup/LobbyButton")?.GetComponent<Button>();
        if (!quitButton) quitButton = transform.Find("Window/ButtonGroup/QuitButton")?.GetComponent<Button>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        if (!Initialized)
        {
            if (dimmerButton) dimmerButton.onClick.AddListener(RequestClose);
            if (continueButton) continueButton.onClick.AddListener(RequestClose);
            if (settingButton) settingButton.onClick.AddListener(OpenSettingFromPause);
            if (lobbyButton) lobbyButton.onClick.AddListener(OnClickLobby);
            if (quitButton) quitButton.onClick.AddListener(OnClickQuit);
            Initialized = true;
        }

        // 시간 멈춤 + 커서 표시 (PausePopup 고유 로직)
        if (Time.timeScale > 0f)
            prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        UpdateLobbyButtonVisibility();
    }

    public void RequestClose()
    {
        if (!gameObject.activeInHierarchy) return;

        // UIBase의 CloseUI() 호출 (애니메이션 시작)
        CloseUI();
    }

    protected override void OnClose()
    {
        bool stageSelectIsOpen = UIManager.Instance.IsUIOpen<StageSelectDialogueUI>();

        // 시간 복원 (PausePopup 고유 로직)
        if (!suppressRestoreOnce)
        {
            // StageSelectDialogueUI가 열려있으면 TimeScale 복원을 억제하고 0으로 유지
            if (stageSelectIsOpen)
            {
                Time.timeScale = 0f; // StageSelectDialogueUI가 TimeScale 0을 유지하도록 명시
            }
            else
            {
                Time.timeScale = prevTimeScale; // 다른 UI가 열리지 않은 경우에만 원래대로 복원
            }
        }
        else
        {
            suppressRestoreOnce = false; // 1회성 플래그 초기화
        }
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
        /*
        //  UIBase의 CloseUI를 호출하기 위해 RequestClose() 사용
        RequestClose();

        // UIBase의 CloseUI 애니메이션 완료(gameObject.activeInHierarchy == false)를 대기
        while (gameObject.activeInHierarchy) yield return null;
        */

        RequestClose();

        if (Time.timeScale == 0f) Time.timeScale = 1f;
        if (!Application.CanStreamedLevelBeLoaded(lobbySceneName))
        {
            Debug.LogError($"씬 '{lobbySceneName}' 없음");
            yield break;
        }
        LoadingScreen.Instance.Load(lobbySceneName);
    }

    private void UpdateLobbyButtonVisibility()
    {
        if (!lobbyButton) return;

        bool isLobby = SceneManager.GetActiveScene().name == lobbySceneName;
        lobbyButton.gameObject.SetActive(!isLobby);
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