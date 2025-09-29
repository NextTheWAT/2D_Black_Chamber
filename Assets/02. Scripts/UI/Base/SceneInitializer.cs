using UnityEngine;
using UnityEngine.SceneManagement;

// ���� ���۵� ��, �� ������ ������ �� UIKey�� UIRoot�� �˷��ִ� �ʱ�ȭ ��ũ��Ʈ.
public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private UIRoot uiRoot;   // UIRoot ������ ����
    [SerializeField] private UIKey activeUI;  // �� ������ ������ Canvas ����

    private void Start()
    {
        if (uiRoot != null)
            uiRoot.ShowOnly(activeUI);
        else
            Debug.LogError("[SceneInitializer] uiRoot ������ ����");

        CursorManager.Instance?.ApplyByUIKey(activeUI);

        BGMManager.Instance?.SetUiContext(activeUI, instant: true);

        var pause = FindFirstObjectByType<PausePopup>(FindObjectsInactive.Include);
        if (pause)
        {
            bool showLobby = (activeUI == UIKey.Game);
            pause.transform.Find("Window/ButtonGroup/LobbyButton")?.gameObject.SetActive(showLobby);
        }

        if (activeUI == UIKey.Game)
        {
            GameStats.Instance.StartStage();
            PlayerPrefs.SetString("LastStage", SceneManager.GetActiveScene().name);
        }

    }
}
