using UnityEngine;
using UnityEngine.SceneManagement;
using Esper.Freeloader;

public class TitleUIController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string lobbySceneName = "LobbyScene";
    [SerializeField] private string tutorialSceneName = "TutorialScene";

    private const string PrefKey_TutorialDone = "TutorialScene";

    public void StartGame()
    {
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);

        const string PrefKey_TutorialDone = "TutorialDone";
        string tutorialSceneName = "TutorialScene";
        string lobbySceneName = "02. LobbyScene";

        // PlayerPrefs에 튜토리얼 완료 여부 확인
        bool tutorialDone = PlayerPrefs.GetInt(PrefKey_TutorialDone, 0) == 1;

        // 완료 안 됐으면 TutorialScene, 완료됐으면 LobbyScene으로 이동
        string nextScene = tutorialDone ? lobbySceneName : tutorialSceneName;

        if (!Application.CanStreamedLevelBeLoaded(nextScene))
        {
            Debug.LogError($"[TitleUIController] 씬 '{nextScene}'을 찾을 수 없음");
            return;
        }

        LoadingScreen.Instance.Load(nextScene);
        //LoadingCanvas.LoadScene(nextScene);
    }

    public void OpenSetting()
    {
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);
        UIManager.Instance.OpenUI<SettingPopup>();
    }

    public void QuitGame()
    {
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);
        Debug.Log("게임 종료");

#if UNITY_EDITOR
        // 에디터에서는 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
