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
    [SerializeField] private Button lobbyButton;   // �κ��
    [SerializeField] private Button quitButton;    // ���� ����

    [Header("Anim")]
    [SerializeField] private float fadeDuration = 0.18f;

    [SerializeField] private string lobbySceneName = "LobbyScene";

    private Coroutine fadeCo;
    private float prevTimeScale = 1f; // ���� �� ������
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
        //gameObject.SetActive(false); // �⺻ ��Ȱ��   �̳༮�� ��������
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

        // �ð� ���� + Ŀ�� ǥ��
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

        // �������� ���� �ѱ�� ��Ȳ�̸� �̹� �� ���� ���� ���� (�Ͻ����� ����)
        if (!suppressRestoreOnce)
            Time.timeScale = prevTimeScale;
        else
            suppressRestoreOnce = false; // 1ȸ�� �÷��� �ʱ�ȭ

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

        // unscaledDeltaTime ���
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
        // 1) ���� ���� ����
        var setting = UIManager.Instance.OpenUI<SettingPopup>();

        // 2) ������ ������ Pause ����� (�ڷ�ƾ���� ����)
        if (setting != null)
            StartCoroutine(ReopenWhenClosed(setting));

        // 3) �̹� ������ �������� ����(�Ͻ����� ����), Pause �ݱ�
        suppressRestoreOnce = true;
        RequestClose();
    }

    private IEnumerator ReopenWhenClosed(SettingPopup setting)
    {
        // SettingPopup�� ��Ȱ��ȭ�� ������ ���
        while (setting != null && setting.gameObject.activeInHierarchy)
            yield return null;

        // ���� ���� �� Pause �ڵ� ����
        UIManager.Instance.OpenUI<PausePopup>();
    }

    private void OnClickLobby()
    {
        StartCoroutine(ReturnToLobby());
    }

    private IEnumerator ReturnToLobby()
    {
        RequestClose();
        while (gameObject.activeInHierarchy) yield return null; // ���� ������ ���
        if (Time.timeScale == 0f) Time.timeScale = 1f;
        if (!Application.CanStreamedLevelBeLoaded(lobbySceneName))
        {
            Debug.LogError($"�� '{lobbySceneName}' ����");
            yield break;
        }
        SceneManager.LoadScene(lobbySceneName);
    }

    private void UpdateLobbyButtonVisibility()
    {
        if (!lobbyButton) return;

        bool isLobby = SceneManager.GetActiveScene().name == lobbySceneName;
        lobbyButton.gameObject.SetActive(!isLobby);   // �κ�� ����, �� �ܿ� ǥ��
                                                      // ���� ���̾ƿ� �����ϰ� ������ ��� ��ü
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
