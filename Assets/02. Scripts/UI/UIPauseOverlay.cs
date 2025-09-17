using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPauseOverlay : MonoBehaviour // 일시정지 화면
{
    [Header("Wiring")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private Button resumeButton;   // 계속하기
    [SerializeField] private Button settingsButton; // 설정
    [SerializeField] private Button lobbyButton;    // 로비
    [SerializeField] private Button quitButton;     // 게임종료

    [Header("Navigation")]
    [SerializeField] private string lobbySceneName = "StageScene"; // 로비 씬

    private bool isPaused = false;
    private float prevTimeScale = 1f;
    private bool prevCursorVisible;
    private CursorLockMode prevCursorLock;

    private void Awake()
    {
        // 버튼 클릭 연결
        if (resumeButton) resumeButton.onClick.AddListener(Resume);
        if (settingsButton) settingsButton.onClick.AddListener(OpenSettings);
        if (lobbyButton) lobbyButton.onClick.AddListener(GoLobby);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);

        // 시작은 비활성화
        if (overlayRoot) overlayRoot.SetActive(false);
    }

    // 외부에서 호출할 수 있는 인터페이스
    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause() // 시간 멈춤
    {
        if (isPaused) return;

        isPaused = true;

        // 현재 TimeScale/커서 상태 백업
        prevTimeScale = Time.timeScale;
        prevCursorVisible = Cursor.visible;
        prevCursorLock = Cursor.lockState;

        Time.timeScale = 0f;

        // UI 표시
        if (overlayRoot) overlayRoot.SetActive(true);

        // 커서 보이기
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        if (overlayRoot) overlayRoot.SetActive(false); // UI 숨김

        // 시간 복구
        Time.timeScale = Mathf.Approximately(prevTimeScale, 0f) ? 1f : prevTimeScale;

        Cursor.visible = prevCursorVisible;
        Cursor.lockState = prevCursorLock;
    }

    private void OpenSettings() // 설정 열기
    {
        Debug.Log("설정");
    }

    private void GoLobby() // 로비로 이동
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(lobbySceneName))
            SceneManager.LoadScene(lobbySceneName);
    }

    private void QuitGame() // 게임 종료
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }
}
