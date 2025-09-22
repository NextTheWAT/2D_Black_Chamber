using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ClearResultUI : MonoBehaviour //클리어씬 결과 채우기/ 버튼 활성화
{
    [Header("Texts")]
    [SerializeField] private TMP_Text KillsText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text rewardText;

    [Header("Buttons")]
    [SerializeField] private Button toLobbyButton;
    [SerializeField] private Button retryButton;

    [Header("Scene Names")]
    [SerializeField] private string lobbySceneName = "StageScene"; //로비로 돌아가기

    private void Awake()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        //버튼 이벤트
        if (toLobbyButton) toLobbyButton.onClick.AddListener(GoLobby);
        if (retryButton) retryButton.onClick.AddListener(RetryMission);
    }

    private void Start() // ClearRunData에 채워진 값으로 UI 갱신
    {
        if (KillsText) KillsText.text = $"제거한 적 : {ClearRunData.Kills}";
        if (stateText) stateText.text = $"클리어 상태 : {(!string.IsNullOrEmpty(ClearRunData.clearState) ? ClearRunData.clearState : "-")}";
        if (timeText) timeText.text = $"소요 시간 : {FormatTime(ClearRunData.elapsedTime)}";
        if (rewardText) rewardText.text = $"+{ClearRunData.reward}$";
    }

    private string FormatTime(float seconds) // 소요시간 > HH:MM:SS
    {
        if (seconds <= 0f)
            return "00:00:00";
        int total = Mathf.FloorToInt(seconds);
        int h = total / 3600;
        int m = (total % 3600) / 60;
        int s = total % 60;
        return $"{h:00}:{m:00}:{s:00}";
    }

    private void GoLobby() //로비로 이동
    {
        SceneManager.LoadScene(lobbySceneName);
    }

    private void RetryMission() //직전 플레이 씬으로 재시작
    {
        string target = string.IsNullOrEmpty(ClearRunData.lastGameplayScene)
            ? "ProtoTypeScene"
            : ClearRunData.lastGameplayScene;

        SceneManager.LoadScene(target);
    }
}
