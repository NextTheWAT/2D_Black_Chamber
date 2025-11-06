using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class CrosshairCursor : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private UIKey uiKey;

    [Header("크로스헤어")]
    [SerializeField] private Canvas canvas;

    [SerializeField] private Image upBar;
    [SerializeField] private Image downBar;
    [SerializeField] private Image leftBar;
    [SerializeField] private Image rightBar;

    private RectTransform rect;
    private RectTransform canvasRect;

    [Header("재장전 UI")]
    public GameObject reload;                 // 재장전 UI 루트
    public Image reloadFillAmount;            // Type=Filled 인 Image
    public float reloadFillDuration = 2f;     // 초 단위
    [SerializeField] private bool useUnscaledTime = false;

    private float cacheElsapsedTime = 0f;
    private Coroutine reloadCR;

    private void OnEnable()
    {
        if (cacheElsapsedTime > 0f)
            PlayReloadUI();
    }

    private void Start()
    {
        // CursorManager에 자신을 등록
        CursorManager.Instance.AddCrosshair(this, uiKey);

        // 안전장치: Type이 Filled가 아니면 강제로 전환
        if (reloadFillAmount != null && reloadFillAmount.type != Image.Type.Filled)
            reloadFillAmount.type = Image.Type.Filled;
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.transform as RectTransform;
        foreach (var g in GetComponentsInChildren<Graphic>(true))
            g.raycastTarget = false;
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        Vector2 screenPos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector2)Input.mousePosition;

        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out var local))
            rect.anchoredPosition = local;

        // UpdateCrosshairSpread();
    }

    // ----- 공개 API: 재장전 UI 시작/중단 -----
    public void PlayReloadUI(float durationSeconds)
    {
        if (reloadCR != null) StopCoroutine(reloadCR);
        reloadCR = StartCoroutine(Co_ReloadFill(Mathf.Max(0.0001f, durationSeconds)));
    }

    public void PlayReloadUI() => PlayReloadUI(reloadFillDuration);

    public void StopReloadUI()
    {
        if (reloadCR != null) StopCoroutine(reloadCR);
        reloadCR = null;
        if (reloadFillAmount) reloadFillAmount.fillAmount = 0f;
        if (reload) reload.SetActive(false);
        SetCrosshairVisible(true);
    }
    // ---------------------------------------

    private IEnumerator Co_ReloadFill(float duration)
    {
        // 준비
        WeaponManager.Instance.isReloading = true;
        if (reloadFillAmount) reloadFillAmount.fillAmount = 0f;
        if (reload) reload.SetActive(true);
        SetCrosshairVisible(false);

        float elapsed = cacheElsapsedTime;
        while (elapsed < duration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            cacheElsapsedTime = elapsed;
            float t = Mathf.Clamp01(elapsed / duration);
            if (reloadFillAmount) reloadFillAmount.fillAmount = t;
            yield return null;
        }

        // 종료
        if (reloadFillAmount) reloadFillAmount.fillAmount = 1f;
        if (reload) reload.SetActive(false);
        SetCrosshairVisible(true);
        cacheElsapsedTime = 0f;
        reloadCR = null;
        WeaponManager.Instance.isReloading = false;
    }

    public void UpdateCrosshairSpread()
    {
        if (WeaponManager.Instance.CurrentWeapon == null) return;

        Shooter shooter = WeaponManager.Instance.CurrentWeapon;
        Vector2 gunPoint = Camera.main.WorldToScreenPoint(shooter.gunPoint.position);
        Vector2 mousePos = Input.mousePosition;

        float distance = Vector2.Distance(gunPoint, mousePos);
        float angle = shooter.CurrentSpread * Mathf.Deg2Rad;

        float delta = distance * Mathf.Tan(angle);
        SetCrosshairDelta(delta * 0.5f);
    }

    public void SetCrosshairDelta(float delta)
    {
        upBar.rectTransform.anchoredPosition = new Vector2(0, delta);
        downBar.rectTransform.anchoredPosition = new Vector2(0, -delta);
        leftBar.rectTransform.anchoredPosition = new Vector2(-delta, 0);
        rightBar.rectTransform.anchoredPosition = new Vector2(delta, 0);
    }

    private void SetCrosshairVisible(bool visible)
    {
        upBar.enabled = visible;
        downBar.enabled = visible;
        leftBar.enabled = visible;
        rightBar.enabled = visible;
    }
}
