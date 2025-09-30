using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIClearResult : UIBase
{
    [Header("Texts")]
    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text rewardText;

    [Header("Buttons")]
    [SerializeField] private Button backToLobbyButton;
    [SerializeField] private Button retryButton;

    [Header("Scenes")]
    [SerializeField] private string lobbySceneName = "LobbyScene";

    private ClearResultData _data;

    public void SetResult(ClearResultData data)
    {
        _data = data;
        Apply();
    }

    private void OnEnable()
    {
        Debug.Log("[UIClearResult] OnEnable");
        if (!Initialized)
        {
            if (backToLobbyButton) backToLobbyButton.onClick.AddListener(GoLobby);
            if (retryButton) retryButton.onClick.AddListener(Retry);
            Initialized = true;
        }

        if (_data == null && TempResultHolder.Data != null)
        {
            SetResult(TempResultHolder.Data);
            TempResultHolder.Data = null;
            return;
        }

        Apply();
    }

    private void Apply()
    {
        if (_data == null) return;

        if (killText) killText.text = $"������ �� : {_data.killCount:00}";
        if (stateText) stateText.text = $"Ŭ���� ���� : {_data.clearStateText}";
        if (timeText) timeText.text = $"�ҿ� �ð� : {Format(_data.elapsedSeconds)}";
        if (rewardText) rewardText.text = $"���� : +{_data.rewardDollar}$";
    }

    private static string Format(float seconds) //�ð� ���
    {
        int s = Mathf.Max(0, Mathf.FloorToInt(seconds));
        int h = s / 3600;
        int m = (s % 3600) / 60;
        int sec = s % 60;
        return $"{h:00}:{m:00}:{sec:00}";
    }

    private void GoLobby()
    {
        Debug.Log("[UIClearResult] GoLobby() clicked");
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(lobbySceneName))
        {
            Debug.LogError("[UIClearResult] lobbySceneName �������");
            return;
        }

        Debug.Log($"[UIClearResult] CurrentScene = {SceneManager.GetActiveScene().name}");
        Debug.Log($"[UIClearResult] Try load lobbySceneName = '{lobbySceneName}'");

        if (!Application.CanStreamedLevelBeLoaded(lobbySceneName))
        {
            Debug.LogError($"[UIClearResult] ���弼�ÿ��� ���� ��ã��: '{lobbySceneName}'");
            return;
        }

        try
        {
            SceneManager.LoadScene(lobbySceneName);
            Debug.Log("[UIClearResult] LoadScene(lobby) ȣ�� �Ϸ�");
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void Retry()
    {
        Debug.Log("[UIClearResult] Retry() clicked");
        Time.timeScale = 1f;

        string lastStage = PlayerPrefs.GetString("LastStage", string.Empty);
        Debug.Log($"[UIClearResult] LastStage(PlayerPrefs) = '{lastStage}'");

        if (string.IsNullOrEmpty(lastStage))
        {
            Debug.LogError("[UIClearResult] ������ �������� ������ ���� (PlayerPrefs 'LastStage' �̱��)");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(lastStage))
        {
            Debug.LogError($"[UIClearResult] ���弼�ÿ��� ���� ��ã��: '{lastStage}'");
            return;
        }

        try
        {
            SceneManager.LoadScene(lastStage);
            Debug.Log("[UIClearResult] LoadScene(lastStage) ȣ�� �Ϸ�");
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }
}
