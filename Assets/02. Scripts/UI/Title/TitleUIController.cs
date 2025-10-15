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

        // PlayerPrefs�� Ʃ�丮�� �Ϸ� ���� Ȯ��
        bool tutorialDone = PlayerPrefs.GetInt(PrefKey_TutorialDone, 0) == 1;

        // �Ϸ� �� ������ TutorialScene, �Ϸ������ LobbyScene���� �̵�
        string nextScene = tutorialDone ? lobbySceneName : tutorialSceneName;

        if (!Application.CanStreamedLevelBeLoaded(nextScene))
        {
            Debug.LogError($"[TitleUIController] �� '{nextScene}'�� ã�� �� ����");
            return;
        }

        LoadingCanvas.LoadScene(nextScene);
    }

    public void OpenSetting()
    {
        UIManager.Instance.OpenUI<SettingPopup>();
    }

    public void QuitGame()
    {
        Debug.Log("���� ����");

#if UNITY_EDITOR
        // �����Ϳ����� �÷��� ��� ����
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
