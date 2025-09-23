using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void OpenSetting()
    {
        Debug.Log("���� ��ư Ŭ����");
    }
    
    public void QuitGame()
    {
        Debug.Log("���� ����");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
