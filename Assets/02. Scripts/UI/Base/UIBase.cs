using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public abstract class UIBase : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float animDuration = 0.3f; // 애니메이션 지속 시간
    [SerializeField] private Ease openEase = Ease.OutBack; // 열릴 때 이징 효과
    [SerializeField] private Ease closeEase = Ease.InBack; // 닫힐 때 이징 효과

    public bool IsOpen { get; protected set; }
    protected bool Initialized;

    [SerializeField] private RectTransform root;    // 비워두면 자동 할당
    [SerializeField] protected CanvasGroup _canvasGroup;

    protected CanvasGroup canvasGroup => _canvasGroup;

    public virtual void OpenUI()
    {
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);

        // 닫기 애니메이션 중이었다면 정지
        DOTween.Kill(root);

        gameObject.SetActive(true);
        EnsureRefs();

        IsOpen = true;

        PlayOpenAnimation();

        // OnOpen 훅은 애니메이션 시작 시 호출
        OnOpen();
    }

    public virtual void CloseUI()
    {
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);
        EnsureRefs();

        // 닫기 애니메이션 실행
        PlayCloseAnimation();
    }

    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }

    private void PlayOpenAnimation()
    {
        // 1. 초기 상태 설정
        if (root) root.localScale = Vector3.one * 0.9f;
        if (canvasGroup) canvasGroup.alpha = 0f;

        // 2. 상호작용성 즉시 활성화 (사용자가 애니메이션 도중 클릭할 수 있도록)
        if (canvasGroup)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        // 3. Scale 애니메이션
        root?.DOScale(Vector3.one, animDuration)
            .SetUpdate(true) // TimeScale=0에서도 작동하도록 설정
            .SetEase(openEase);

        // 4. Alpha 애니메이션
        canvasGroup?.DOFade(1f, animDuration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                // 애니메이션 완료 후 CanvasGroup 알파 값 1 보장
                if (canvasGroup) canvasGroup.alpha = 1f;
            });
    }

    private void PlayCloseAnimation()
    {
        // 1. 상호작용성 즉시 비활성화 (닫는 애니메이션 중 클릭 방지)
        if (canvasGroup)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // 2. Scale 애니메이션
        root?.DOScale(Vector3.one * 0.9f, animDuration * 0.7f) // 닫을 땐 조금 더 빠르게
            .SetUpdate(true)
            .SetEase(closeEase);

        // 3. Alpha 애니메이션 및 완료 처리
        canvasGroup?.DOFade(0f, animDuration * 0.7f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                // 애니메이션 완료 후 OnClose 훅 호출
                OnClose();

                IsOpen = false;

                // 최종적으로 GameObject 비활성화 (UIManager의 CloseUI 역할)
                gameObject.SetActive(false);
            });
    }

    protected void EnsureRefs()
    {
        // root가 없으면 RectTransform으로 자동 할당 시도
        if (!root) root = transform as RectTransform;
        if (!canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
}