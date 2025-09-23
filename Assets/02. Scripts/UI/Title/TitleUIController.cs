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
        Debug.Log("설정 버튼 클릭함");
    }
    
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
