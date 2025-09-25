using UnityEngine;

// 씬이 시작될 때, 이 씬에서 보여야 할 UIKey를 UIRoot에 알려주는 초기화 스크립트.
public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private UIRoot uiRoot;   // UIRoot 프리팹 참조
    [SerializeField] private UIKey activeUI;  // 이 씬에서 보여줄 Canvas 선택

    private void Start()
    {
        if (uiRoot != null)
            uiRoot.ShowOnly(activeUI);
        else
            Debug.LogError("[SceneInitializer] uiRoot 참조가 비였음");

        BGMManager.Instance?.SetUiContext(activeUI, instant: true);

        var pause = FindFirstObjectByType<PausePopup>(FindObjectsInactive.Include);
        if (pause)
        {
            bool showLobby = (activeUI == UIKey.Game);
            pause.transform.Find("Window/ButtonGroup/LobbyButton")?.gameObject.SetActive(showLobby);
        }
    }
}
