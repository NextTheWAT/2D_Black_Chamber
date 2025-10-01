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
    [SerializeField] private RectTransform root;   // ����θ� �ڵ� �Ҵ�
    [SerializeField] protected CanvasGroup _canvasGroup; 
    [SerializeField] private Image vignette;       // ����(��� ��). ���� ���Ʈ �̹���

    [Header("Tween: Options")]
    [SerializeField] private bool microJitterOnOpen = true; // ��¦ ��ģ ����
    [SerializeField] private float jitterStrength = 3f;     // ��Ŀ ������ ������ ����

    protected CanvasGroup canvasGroup => _canvasGroup;

    Sequence _openSeq, _closeSeq;

    // ����������������������������������������������������������������������������������������������������������������������������������������������������������

    public virtual void OpenUI()
    {
        gameObject.SetActive(true);
        EnsureRefs();

        IsOpen = true;

        // ���� �ǹ� ����: �ٷ� �� ȣ��
        OnOpen();

        if (!enableTween) return;
        PlayOpenTween();
    }

    public virtual void CloseUI()
    {
        EnsureRefs();

        // ���� �ǹ� ����: �ݱ� ���� �� �� ȣ��
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

    // ����������������������������������������������������������������������������������������������������������������������������������������������������������
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

        // ���� ���̵�
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

        // ���� ���̵�ƿ�
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
        // ��Ȱ�� �� �ܿ� Ʈ�� ����(��Ȱ�� �� �ʱ���� �缼��)
        KillAll();
    }

    void OnDestroy()
    {
        KillAll();
    }
}
