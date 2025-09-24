using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string lobbySceneName = "LobbyScene";

    public void StartGame()
    {
        if (!Application.CanStreamedLevelBeLoaded(lobbySceneName))
        {
            Debug.LogError($"[TitleUIController] 씬 '{lobbySceneName}'을 찾을 수 없음");
            return;
        }

        SceneManager.LoadScene(lobbySceneName);
    }

    public void OpenSetting()
    {
        UIManager.Instance.OpenUI<SettingPopup>();
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");

#if UNITY_EDITOR
        // 에디터에서는 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
