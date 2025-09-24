using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIGameOVer : UIBase
{
    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button lobbyButton;

    [Header("Scenes")]
    [SerializeField] private string lobbySceneName = "LobbyScene";

    private void OnEnable()
    {
        if (!Initialized)
        {
            if (retryButton) retryButton.onClick.AddListener(OnClickRetry);
            if (lobbyButton) lobbyButton.onClick.AddListener(OnClickLobby);
            Initialized = true;
        }

        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void OnClickLobby()
    {
        Debug.Log("[UIGameOver] Lobby clicked");
        if (string.IsNullOrEmpty(lobbySceneName))
        {
            Debug.LogError("[UIGameOver] lobbySceneName �������");
            return;
        }
        if (!Application.CanStreamedLevelBeLoaded(lobbySceneName))
        {
            Debug.LogError($"[UIGameOver] ���弼�ÿ��� ���� ��ã��: '{lobbySceneName}'");
            return;
        }
        SceneManager.LoadScene(lobbySceneName);
    }

    private void OnClickRetry()
    {
        Debug.Log("[UIGameOver] Retry clicked");

        string lastStage = PlayerPrefs.GetString("LastStage", string.Empty);
        if (string.IsNullOrEmpty(lastStage))
        {
            Debug.LogError("[UIGameOver] LastStage ����");
            return;
        }
        if (!Application.CanStreamedLevelBeLoaded(lastStage))
        {
            Debug.LogError($"[UIGameOver] ���弼�ÿ��� ���� ��ã��");
            return;
        }
        SceneManager.LoadScene(lastStage);
    }
}
