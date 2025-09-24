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

        if (killText) killText.text = $"제거한 적 : {_data.killCount:00}";
        if (stateText) stateText.text = $"클리어 상태 : {_data.clearStateText}";
        if (timeText) timeText.text = $"소요 시간 : {Format(_data.elapsedSeconds)}";
        if (rewardText) rewardText.text = $"보상 : +{_data.rewardDollar}$";
    }

    private static string Format(float seconds) //시간 계산
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

    private void Retry() // 마지막 플레이한 스테이지 불러오기
    {
        Time.timeScale = 1f;

        string lastStage = PlayerPrefs.GetString("LastStage", string.Empty);
        if (!string.IsNullOrEmpty(lastStage))
            SceneManager.LoadScene(lastStage);
        else
            Debug.LogError("[UIClearResult] 마지막 스테이지 정보가 없음");
    }
}
