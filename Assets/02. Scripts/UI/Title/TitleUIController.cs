using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string lobbySceneName = "LobbyScene";
    [SerializeField] private string tutorialSceneName = "TutorialScene";

    private const string PrefKey_TutorialDone = "TutorialScene";

    public void StartGame()
    {
        const string PrefKey_TutorialDone = "TutorialDone";
        string tutorialSceneName = "TutorialScene";
        string lobbySceneName = "02. LobbyScene";

        bool tutorialDone = PlayerPrefs.GetInt(PrefKey_TutorialDone, 0) == 1;
        string nextScene = tutorialDone ? lobbySceneName : tutorialSceneName;

        if (!Application.CanStreamedLevelBeLoaded(nextScene))
        {
            Debug.LogError($"[TitleUIController] 씬 '{nextScene}'을 찾을 수 없음");
            return;
        }

        // 튜토리얼로 들어갈 때만 시작 이벤트 1회 기록
        if (!tutorialDone)
            GA.Tutorial_Start();

        LoadingCanvas.LoadScene(nextScene);
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
