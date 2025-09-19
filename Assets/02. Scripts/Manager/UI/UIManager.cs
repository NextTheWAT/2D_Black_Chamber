using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Scene Routing")]
    [SerializeField] private string titleSceneName = "TitleScene";
    [SerializeField] private string[] lobbyScenes = { "StageScene" }; //�κ��
    [SerializeField] private string[] gameplaySceneNames = { };
    [SerializeField] private string[] gameplayScenePrefixes = { "ProtoTypeScene" };

    [Header("Result Scenes")] //�����
    [SerializeField] private string[] resultScenes = { "ClearScene", "GameOverScene" };
    [SerializeField] private string[] resultScenePrefixes = { "ResultScene" };

    [Header("UI Groups")]
    [SerializeField] private GameObject titleCanvas;
    [SerializeField] private GameObject lobbyGroup;
    [SerializeField] private GameObject playerGroup;
    [SerializeField] private GameObject crosshairCanvas;
    [SerializeField] private GameObject gameGroup;
    [SerializeField] private GameObject clearCanvas;

    [Header("Pause Canvases (optional)")]
    [SerializeField] private GameObject lobbyPauseCanvas;
    [SerializeField] private GameObject gamePauseCanvas;

    [Header("System")]
    [SerializeField] private EventSystem eventSystem;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EnsureSingleEventSystem(); //EventSystem �� ���� ����
        ApplyLayout(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyLayout(scene.name);
    }

    private void ApplyLayout(string sceneName) //���� �� �̸� �������� � UI �������� ����
    {
        bool isTitle = sceneName == titleSceneName;
        bool isLobby = Matches(sceneName, lobbyScenes);
        bool isGameplay = !isTitle && !isLobby &&
            (Matches(sceneName, gameplaySceneNames) || 
            StartsWithAny(sceneName, gameplayScenePrefixes));
        bool isResult = Matches(sceneName, resultScenes) || StartsWithAny(sceneName, resultScenePrefixes);

        // Ÿ��Ʋ�� ����
        if (titleCanvas) titleCanvas.SetActive(isTitle);

        // �κ�� ���� (StageScene)
        if (lobbyGroup) lobbyGroup.SetActive(isLobby);

        // ���Ӿ� ����
        if (playerGroup) playerGroup.SetActive(isGameplay);
        if (crosshairCanvas) crosshairCanvas.SetActive(isGameplay);
        if (gameGroup) gameGroup.SetActive(isGameplay);

        //����� ����
        if (clearCanvas) clearCanvas.SetActive(isResult);

        if (isResult)
        {
            titleCanvas?.SetActive(false);
            lobbyGroup?.SetActive(false);
            playerGroup?.SetActive(false);
            crosshairCanvas?.SetActive(false);
            gameGroup?.SetActive(false);
        }

        // �Ͻ������� �⺻ ���� (�ʿ��� �� �ܺο��� �ѱ�)
        if (lobbyPauseCanvas) lobbyPauseCanvas.SetActive(false);
        if (gamePauseCanvas) gamePauseCanvas.SetActive(false);
    }

    private bool Matches(string sceneName, string[] list)
    {
        if (list == null) return false;
        for (int i = 0; i < list.Length; i++)
            if (!string.IsNullOrEmpty(list[i]) && sceneName == list[i]) return true;
        return false;
    }

    private bool StartsWithAny(string sceneName, string[] prefixes)
    {
        if (prefixes == null) return false;
        for (int i = 0; i < prefixes.Length; i++)
            if (!string.IsNullOrEmpty(prefixes[i]) && sceneName.StartsWith(prefixes[i]))
                return true;
        return false;
    }

    private void EnsureSingleEventSystem()
    {
        var all = FindObjectsOfType<EventSystem>(true);
        if (all.Length <= 1) return;

        foreach (var es in all)
        {
            bool isMine = eventSystem && es == eventSystem;
            bool isChildOfMine = es && es.transform.IsChildOf(transform);
            if (!isMine && !isChildOfMine)
                Destroy(es.gameObject); //EventSystem 1���� ����
        }
    }

    public UIPauseOverlay GetPauseOverlayForCurrentMode()
    {
        // �κ��� ��
        if (lobbyGroup && lobbyGroup.activeInHierarchy && lobbyPauseCanvas)
            return lobbyPauseCanvas.GetComponent<UIPauseOverlay>();

        // �����÷����� ��
        if (gameGroup && gameGroup.activeInHierarchy && gamePauseCanvas)
            return gamePauseCanvas.GetComponent<UIPauseOverlay>();

        return null;
    }

}
