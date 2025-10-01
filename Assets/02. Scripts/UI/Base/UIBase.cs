using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public abstract class UIBase : MonoBehaviour
{
    public bool IsOpen { get; private set; }
    protected bool Initialized;

    [Header("Tween: Common")]
    [SerializeField] private bool enableTween = true;
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private float openDuration = 0.35f;
    [SerializeField] private float closeDuration = 0.22f;

    [Header("Tween: Refs")]
    [SerializeField] private RectTransform root;   // 비워두면 자동 할당
    [SerializeField] protected CanvasGroup _canvasGroup; 
    [SerializeField] private Image vignette;       // 선택(없어도 됨). 검은 비네트 이미지

    [Header("Tween: Options")]
    [SerializeField] private bool microJitterOnOpen = true; // 살짝 거친 느낌
    [SerializeField] private float jitterStrength = 3f;     // 앵커 포지션 잔진동 강도

    protected CanvasGroup canvasGroup => _canvasGroup;

    Sequence _openSeq, _closeSeq;

    // ─────────────────────────────────────────────────────────────────────────────

    public virtual void OpenUI()
    {
        gameObject.SetActive(true);
        EnsureRefs();

        IsOpen = true;

        // 원래 의미 보존: 바로 훅 호출
        OnOpen();

        if (!enableTween) return;
        PlayOpenTween();
    }

    public virtual void CloseUI()
    {
        EnsureRefs();

        // 원래 의미 보존: 닫기 시작 시 훅 호출
        OnClose();

        IsOpen = false;

        if (!enableTween)
        {
            gameObject.SetActive(false);
            return;
        }
        PlayCloseTween();
    }

    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }

    // ─────────────────────────────────────────────────────────────────────────────
    void EnsureRefs()
    {
        if (!root) root = transform as RectTransform;
        if (!canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void KillAll()
    {
        _openSeq?.Kill();
        _closeSeq?.Kill();
        _openSeq = _closeSeq = null;
    }

    void PlayOpenTween()
    {
        KillAll();

        canvasGroup.alpha = 0f;
        root.localScale = Vector3.one * 0.98f;
        root.anchoredPosition = new Vector2(0f, -40f);
        if (vignette) vignette.color = new Color(0f, 0f, 0f, 0.6f);


        _openSeq = DOTween.Sequence().SetUpdate(useUnscaledTime);

        // 공통 페이드
        _openSeq.Join(canvasGroup.DOFade(1f, openDuration));


        _openSeq.Join(root.DOAnchorPos(Vector2.zero, openDuration).SetEase(Ease.OutCubic));
        _openSeq.Join(root.DOScale(1f, openDuration).SetEase(Ease.OutQuad));
        if (microJitterOnOpen)
            _openSeq.Join(root.DOShakeAnchorPos(0.22f, jitterStrength, vibrato: 12, randomness: 60, snapping: false, fadeOut: true));
        if (vignette) _openSeq.Join(vignette.DOFade(0.35f, 0.18f));


        _openSeq.OnComplete(() => { _openSeq = null; });
    }

    void PlayCloseTween()
    {
        KillAll();

        _closeSeq = DOTween.Sequence().SetUpdate(useUnscaledTime);

        // 공통 페이드아웃
        _closeSeq.Join(canvasGroup.DOFade(0f, closeDuration));

        _closeSeq.Join(root.DOAnchorPos(new Vector2(0f, -20f), closeDuration).SetEase(Ease.InCubic));
        _closeSeq.Join(root.DOScale(0.985f, closeDuration).SetEase(Ease.InCubic));
        if (vignette) _closeSeq.Join(vignette.DOFade(0.0f, closeDuration * 0.8f));


        _closeSeq.OnComplete(() =>
        {
            _closeSeq = null;
            gameObject.SetActive(false);
        });
    }

    void OnDisable()
    {
        // 비활성 시 잔여 트윈 정리(재활성 시 초기상태 재세팅)
        KillAll();
    }

    void OnDestroy()
    {
        KillAll();
    }
}
