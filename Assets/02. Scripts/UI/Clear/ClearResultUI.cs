using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ClearResultUI : MonoBehaviour //Ŭ����� ��� ä���/ ��ư Ȱ��ȭ
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
    [SerializeField] private string lobbySceneName = "StageScene"; //�κ�� ���ư���

    private void Awake()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        //��ư �̺�Ʈ
        if (toLobbyButton) toLobbyButton.onClick.AddListener(GoLobby);
        if (retryButton) retryButton.onClick.AddListener(RetryMission);
    }

    private void Start() // ClearRunData�� ä���� ������ UI ����
    {
        if (KillsText) KillsText.text = $"������ �� : {ClearRunData.Kills}";
        if (stateText) stateText.text = $"Ŭ���� ���� : {(!string.IsNullOrEmpty(ClearRunData.clearState) ? ClearRunData.clearState : "-")}";
        if (timeText) timeText.text = $"�ҿ� �ð� : {FormatTime(ClearRunData.elapsedTime)}";
        if (rewardText) rewardText.text = $"+{ClearRunData.reward}$";
    }

    private string FormatTime(float seconds) // �ҿ�ð� > HH:MM:SS
    {
        if (seconds <= 0f)
            return "00:00:00";
        int total = Mathf.FloorToInt(seconds);
        int h = total / 3600;
        int m = (total % 3600) / 60;
        int s = total % 60;
        return $"{h:00}:{m:00}:{s:00}";
    }

    private void GoLobby() //�κ�� �̵�
    {
        SceneManager.LoadScene(lobbySceneName);
    }

    private void RetryMission() //���� �÷��� ������ �����
    {
        string target = string.IsNullOrEmpty(ClearRunData.lastGameplayScene)
            ? "ProtoTypeScene"
            : ClearRunData.lastGameplayScene;

        SceneManager.LoadScene(target);
    }
}
