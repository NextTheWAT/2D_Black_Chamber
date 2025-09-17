using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPauseOverlay : MonoBehaviour // �Ͻ����� ȭ��
{
    [Header("Wiring")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private Button resumeButton;   // ����ϱ�
    [SerializeField] private Button settingsButton; // ����
    [SerializeField] private Button lobbyButton;    // �κ�
    [SerializeField] private Button quitButton;     // ��������

    [Header("Navigation")]
    [SerializeField] private string lobbySceneName = "StageScene"; // �κ� ��

    private bool isPaused = false;
    private float prevTimeScale = 1f;
    private bool prevCursorVisible;
    private CursorLockMode prevCursorLock;

    private void Awake()
    {
        // ��ư Ŭ�� ����
        if (resumeButton) resumeButton.onClick.AddListener(Resume);
        if (settingsButton) settingsButton.onClick.AddListener(OpenSettings);
        if (lobbyButton) lobbyButton.onClick.AddListener(GoLobby);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);

        // ������ ��Ȱ��ȭ
        if (overlayRoot) overlayRoot.SetActive(false);
    }

    // �ܺο��� ȣ���� �� �ִ� �������̽�
    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause() // �ð� ����
    {
        if (isPaused) return;

        isPaused = true;

        // ���� TimeScale/Ŀ�� ���� ���
        prevTimeScale = Time.timeScale;
        prevCursorVisible = Cursor.visible;
        prevCursorLock = Cursor.lockState;

        Time.timeScale = 0f;

        // UI ǥ��
        if (overlayRoot) overlayRoot.SetActive(true);

        // Ŀ�� ���̱�
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        if (overlayRoot) overlayRoot.SetActive(false); // UI ����

        // �ð� ����
        Time.timeScale = Mathf.Approximately(prevTimeScale, 0f) ? 1f : prevTimeScale;

        Cursor.visible = prevCursorVisible;
        Cursor.lockState = prevCursorLock;
    }

    private void OpenSettings() // ���� ����
    {
        Debug.Log("����");
    }

    private void GoLobby() // �κ�� �̵�
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(lobbySceneName))
            SceneManager.LoadScene(lobbySceneName);
    }

    private void QuitGame() // ���� ����
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
