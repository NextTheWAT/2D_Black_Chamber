using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections; // Coroutine 사용을 위해 추가

// CanvasGroup 컴포넌트가 필요하도록 명시적으로 설정
[RequireComponent(typeof(CanvasGroup))]
public class TutorialPopup2D : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private SpriteRenderer imageRenderer;
    [SerializeField] private SpriteRenderer dimBackground;

    // 애니메이션 대상 참조 필드 추가 및 수정
    [SerializeField] private Transform popupRoot;           // 팝업 이미지/UI를 감싸는 부모 Transform (Scale 대상)
    [SerializeField] private CanvasGroup popupCanvasGroup; // CanvasGroup 참조 (Fade 대상)

    [Header("페이지 리스트")]
    [SerializeField] private List<Sprite> pages = new();

    [Header("애니메이션 설정")] // 애니메이션 관련 필드 추가
    [SerializeField] private float animDuration = 0.3f;
    [SerializeField] private float startScale = 0.8f;

    [Header("마지막 동작")]
    [SerializeField] private string loadSceneAfterLastPage = ""; //비워두면 닫기만함

    public System.Action onClosed;

    int index = 0;
    bool isOpen = false;
    private bool isAnimating = false; // 애니메이션 중 입력 방지 플래그

    private void Awake()
    {
        // 참조 초기화
        if (popupCanvasGroup == null) popupCanvasGroup = GetComponent<CanvasGroup>();
        if (popupRoot == null && imageRenderer != null) popupRoot = imageRenderer.transform.parent;
        if (popupRoot == null) popupRoot = transform;

        if (imageRenderer == null)
            Debug.LogWarning("[TutorialPopup2D] imageRenderer가 비어있습니다.");
        if (pages.Count == 0)
            Debug.LogWarning("[TutorialPopup2D] pages가 비어있습니다.");

        Open();
    }

    private void Update()
    {
        // isOpen이거나, 애니메이션 중이 아닐 때만 입력 허용
        if (!isOpen || isAnimating || Keyboard.current == null) return;

        if (Keyboard.current.fKey.wasPressedThisFrame) //F키로 누르기
            Next();
    }

    void Open()
    {
        isOpen = true;
        SetPage(0);

        if (dimBackground != null) dimBackground.enabled = true;

        // 팝업 열기 애니메이션 시작
        StartCoroutine(PlayOpenAnimation());
    }

    // 팝업 등장 애니메이션 코루틴 (Scale In & Fade In)
    private IEnumerator PlayOpenAnimation()
    {
        isAnimating = true;

        // 1. 초기 상태 설정
        if (popupRoot) popupRoot.localScale = Vector3.one * startScale;
        if (popupCanvasGroup)
        {
            popupCanvasGroup.alpha = 0f;
            // 애니메이션 중에도 Raycast 차단 해제 (혹시 모를 상황 대비)
            popupCanvasGroup.blocksRaycasts = false;
        }

        float timer = 0f;

        // 2. 애니메이션 반복 루프
        while (timer < animDuration)
        {
            float t = timer / animDuration;

            // Lerp (0.8 -> 1.0)
            if (popupRoot)
                popupRoot.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one, t);

            // Lerp (0.0 -> 1.0)
            if (popupCanvasGroup)
                popupCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            timer += Time.unscaledDeltaTime; // TimeScale=0 이어도 작동하도록 Unscaled 사용
            yield return null;
        }

        // 3. 최종 상태 보장 및 입력 활성화
        if (popupRoot) popupRoot.localScale = Vector3.one;
        if (popupCanvasGroup)
        {
            popupCanvasGroup.alpha = 1f;
            popupCanvasGroup.blocksRaycasts = true; // 최종적으로 Raycast 차단 복원
        }

        isAnimating = false;
    }

    void SetPage(int i)
    {
        index = Mathf.Clamp(i, 0, Mathf.Max(0, pages.Count - 1));
        if (imageRenderer != null && pages.Count > 0)
            imageRenderer.sprite = pages[index];
    }

    void Next()
    {
        if (index < pages.Count - 1)
        {
            SetPage(index + 1);
            return;
        }

        if (!string.IsNullOrEmpty(loadSceneAfterLastPage))
        {
            SceneManager.LoadScene(loadSceneAfterLastPage);
        }
        else
        {
            Close();
        }
    }

    public void Close()
    {
        if (!isOpen || isAnimating) return;

        // 닫기 애니메이션 시작
        StartCoroutine(PlayCloseAnimation());
    }

    // 팝업 퇴장 애니메이션 코루틴 (Scale Down & Fade Out)
    private IEnumerator PlayCloseAnimation()
    {
        isAnimating = true;

        float timer = 0f;
        float closeDuration = animDuration * 0.7f; // 닫는 건 좀 더 빠르게

        // 시작 상태 저장
        Vector3 initialScale = popupRoot ? popupRoot.localScale : Vector3.one;
        float initialAlpha = popupCanvasGroup ? popupCanvasGroup.alpha : 1f;

        // 닫기 시작 시 Raycast 즉시 차단 해제 (입력 막기)
        if (popupCanvasGroup) popupCanvasGroup.blocksRaycasts = false;

        // 1. 애니메이션 반복 루프
        while (timer < closeDuration)
        {
            float t = timer / closeDuration;

            // Lerp (현재 크기 -> 0.8)
            if (popupRoot)
                popupRoot.localScale = Vector3.Lerp(initialScale, Vector3.one * startScale, t);

            // Lerp (1 -> 0)
            if (popupCanvasGroup)
                popupCanvasGroup.alpha = Mathf.Lerp(initialAlpha, 0f, t);

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // 2. 최종 정리
        if (dimBackground != null) dimBackground.enabled = false;

        onClosed?.Invoke();

        // Destroy는 반드시 코루틴 완료 후 실행
        Destroy(gameObject);

        isAnimating = false;
    }
}