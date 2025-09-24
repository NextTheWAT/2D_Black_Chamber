using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.Timeline.Actions;

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

    protected override void OnOpen()
    {
        if (!Initialized)
        {
            if (backToLobbyButton) backToLobbyButton.onClick.AddListener(GoLobby);
            if (retryButton) retryButton.onClick.AddListener(Retry);
            Initialized = true;
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
        Time.timeScale = 1f;
        if (string.IsNullOrEmpty(lobbySceneName)) return;
        SceneManager.LoadScene(lobbySceneName);
    }

    private void Retry() // ������ �÷����� �������� �ҷ�����
    {
        Time.timeScale = 1f;

        string lastStage = PlayerPrefs.GetString("LastStage", string.Empty);
        if (!string.IsNullOrEmpty(lastStage))
            SceneManager.LoadScene(lastStage);
        else
            Debug.LogError("[UIClearResult] ������ �������� ������ ����");
    }
}
