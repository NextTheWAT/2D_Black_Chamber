using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class LoadingCanvas : UIBase
{
    public static LoadingCanvas Instance { get; private set; }
    public static Action<bool> OnLoading;

    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Canvas overlayCanvas;   // 로딩 UI용 캔버스
    [SerializeField] private Camera loadingCamera;   // 전용 카메라

    [Header("Timings")]
    [SerializeField] private float fadeIn = 0.25f;
    [SerializeField] private float fadeOut = 0.20f;
    [SerializeField] private float minShowTime = 0.50f;

    [Header("Options")]
    [SerializeField] private bool keepAcrossScenes = true;
    [Tooltip("필요 시 로딩 중 다른 카메라를 잠시 꺼서 깔끔히 가립니다.")]
    [SerializeField] private bool disableOtherCameras = false;

    private bool isLoading;
    private float shownAt;

    // 다른 카메라 상태 백업용
    private Camera[] cachedOthers;
    private bool[] cachedEnable;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (keepAcrossScenes) DontDestroyOnLoad(gameObject);

        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        // 👇 로딩 캔버스를 전용 카메라에 붙여 렌더(씬 카메라와 완전 분리)
        if (overlayCanvas)
        {
            overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            overlayCanvas.worldCamera = loadingCamera;
            overlayCanvas.sortingOrder = 50000;
        }
        if (loadingCamera)
        {
            loadingCamera.enabled = false;   // 기본은 꺼두고, 열릴 때만 켜기
            loadingCamera.depth = 1000;      // Built-in
            // URP 사용 시: Camera Data -> Render Type: Base, Priority: 1000 이상
        }

        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    // === 외부 API ===
    public static Coroutine LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (!Instance)
        {
            Debug.LogError("[LoadingCanvas] 인스턴스가 없습니다.");
            return null;
        }
        if (!Instance.gameObject.activeSelf) Instance.gameObject.SetActive(true);
        if (!Instance.enabled) Instance.enabled = true;

        // (선택) 현재 씬 UI 잠깐 끄기
        var root = FindFirstObjectByType<UIRoot>(FindObjectsInactive.Include);
        root?.HideAllSceneUIs(false);

        return Instance.StartCoroutine(Instance.CoLoad(sceneName, mode));
    }

    private IEnumerator CoLoad(string sceneName, LoadSceneMode mode)
    {
        if (isLoading) yield break;
        isLoading = true;
        OnLoading?.Invoke(true);

        // 로딩 표시
        OpenUI();                  // UIBase: CanvasGroup 세팅
        EnableLoadingCamera(true); // ⭐ 전용 카메라 켜기

        yield return FadeTo(1f, fadeIn);
        shownAt = Time.unscaledTime;

        var op = SceneManager.LoadSceneAsync(sceneName, mode);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f) yield return null;

        float remain = minShowTime - (Time.unscaledTime - shownAt);
        if (remain > 0f) yield return new WaitForSecondsRealtime(remain);

        op.allowSceneActivation = true;

        while (isLoading) yield return null; // sceneLoaded에서 페이드아웃/정리
    }

    private void HandleSceneLoaded(Scene s, LoadSceneMode m)
    {
        StartCoroutine(CoFadeOutAfterLoad());
    }

    private IEnumerator CoFadeOutAfterLoad()
    {
        yield return null; // 한 프레임 양보
        yield return FadeTo(0f, fadeOut);
        EnableLoadingCamera(false); // 전용 카메라 끄기
        CloseUI();                  // UIBase가 비활성 처리
        isLoading = false;
        OnLoading?.Invoke(false);
    }

    protected override void OnOpen()
    {
        if (canvasGroup) canvasGroup.blocksRaycasts = true;
        if (disableOtherCameras) CacheAndDisableOtherCameras();
    }

    protected override void OnClose()
    {
        if (canvasGroup) canvasGroup.blocksRaycasts = false;
        if (disableOtherCameras) RestoreOtherCameras();
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        if (duration <= 0f) { canvasGroup.alpha = target; yield break; }
        float t = 0f, start = canvasGroup.alpha;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        canvasGroup.alpha = target;
    }

    // --- 카메라 제어 ---
    private void EnableLoadingCamera(bool on)
    {
        if (!loadingCamera) return;
        loadingCamera.enabled = on;
        if (on == false) return;
        // 안전하게 최상단에 그리기
        loadingCamera.depth = 1000;
        // URP라면: Camera Data -> Priority 크게
    }

    private void CacheAndDisableOtherCameras()
    {
        cachedOthers = Camera.allCameras;
        cachedEnable = new bool[cachedOthers.Length];
        for (int i = 0; i < cachedOthers.Length; i++)
        {
            cachedEnable[i] = cachedOthers[i].enabled;
            if (cachedOthers[i] != loadingCamera) cachedOthers[i].enabled = false;
        }
    }

    private void RestoreOtherCameras()
    {
        if (cachedOthers == null) return;
        for (int i = 0; i < cachedOthers.Length; i++)
            if (cachedOthers[i]) cachedOthers[i].enabled = cachedEnable[i];
        cachedOthers = null;
        cachedEnable = null;
    }
}
