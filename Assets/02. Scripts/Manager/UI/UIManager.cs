using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Crosshair")]
    [SerializeField] private CrosshairUI crosshairUI;

    public static UIManager Instance { get; private set; } //싱클톤 추가

    public void PulseCrosshair() => crosshairUI?.Pulse();

    //어떤 씬이든 UI_Root가 없으면 자동 생성
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureUIRoot()
    {
        if (FindObjectOfType<UIManager>(true) != null) return; 
        var prefab = Resources.Load<GameObject>("UI_Root");
        if (prefab) Object.Instantiate(prefab);
        else Debug.LogWarning("[UIManager] Resources/UI_Root.prefab 을 찾을 수 없습니다.");
    }

    [Header("Scene Routing")]
    [SerializeField] private string titleSceneName = "TitleScene";
    [SerializeField] private string[] lobbyScenes = { "StageScene" }; //로비씬
    [SerializeField] private string[] gameplaySceneNames = { };
    [SerializeField] private string[] gameplayScenePrefixes = { "ProtoTypeScene" };

    [Header("Result Scenes")] //결과씬
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
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        EnsureSingleEventSystem(); //EventSystem 한 개만 유지
        ApplyLayout(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
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

    private void ApplyLayout(string sceneName) //현재 씬 이름 기준으로 어떤 UI 보여줄지 결정
    {
        bool isTitle = sceneName == titleSceneName;
        bool isLobby = Matches(sceneName, lobbyScenes);
        bool isGameplay = !isTitle && !isLobby &&
            (Matches(sceneName, gameplaySceneNames) || 
            StartsWithAny(sceneName, gameplayScenePrefixes));
        bool isResult = Matches(sceneName, resultScenes) || StartsWithAny(sceneName, resultScenePrefixes);

        // 타이틀씬 전용
        if (titleCanvas) titleCanvas.SetActive(isTitle);

        // 로비씬 전용 (StageScene)
        if (lobbyGroup) lobbyGroup.SetActive(isLobby);

        // 게임씬 전용
        if (playerGroup) playerGroup.SetActive(isGameplay);
        if (crosshairCanvas) crosshairCanvas.SetActive(isGameplay);
        if (gameGroup) gameGroup.SetActive(isGameplay);

        //결과씬 전용
        if (clearCanvas) clearCanvas.SetActive(isResult);

        if (isResult)
        {
            titleCanvas?.SetActive(false);
            lobbyGroup?.SetActive(false);
            playerGroup?.SetActive(false);
            crosshairCanvas?.SetActive(false);
            gameGroup?.SetActive(false);
        }

        // 일시정지는 기본 끄기 (필요할 때 외부에서 켜기)
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
            if (!string.IsNullOrEmpty(prefixes[i]) && sceneName.StartsWith(prefixes[i])) return true;
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
            if (!isMine && !isChildOfMine) Destroy(es.gameObject);
        }
    }

    public UIPauseOverlay GetPauseOverlayForCurrentMode()
    {
        // 로비 모드
        if (lobbyGroup && lobbyGroup.activeInHierarchy && lobbyPauseCanvas)
            return lobbyPauseCanvas.GetComponent<UIPauseOverlay>();
        // 게임 모드
        if (gameGroup && gameGroup.activeInHierarchy && gamePauseCanvas)
            return gamePauseCanvas.GetComponent<UIPauseOverlay>();
        return null;
    }
}
