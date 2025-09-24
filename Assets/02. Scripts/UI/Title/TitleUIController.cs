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
            Debug.LogError($"[TitleUIController] �� '{lobbySceneName}'�� ã�� �� ����");
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
        Debug.Log("���� ����");

#if UNITY_EDITOR
        // �����Ϳ����� �÷��� ��� ����
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Update()
    {
        //ESC�� ���� �˾� �ݱ�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var popup = Object.FindFirstObjectByType<SettingPopup>(FindObjectsInactive.Include);
            if (popup != null && popup.gameObject.activeInHierarchy)
                popup.RequestClose();
        }
    }
}
