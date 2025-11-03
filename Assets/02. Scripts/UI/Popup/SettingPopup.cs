using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : UIBase
{
    [Header("Refs")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button dimmerButton;

    [Header("Animation")]
    [SerializeField] private Image animationImage; // 애니메이션 이미지
    [SerializeField] private Sprite[] animationFrames; // 애니메이션 프레임 담을 배열
    [SerializeField] private float frameDuration = 0.1f;

    [Header("Content")]
    [SerializeField] private GameObject contentRoot; // 애니메이션 후 활성화할 UI

    private Coroutine _animationCo; // 애니메이션 코루틴 참조

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Reset()
    {
        if (closeButton == null) closeButton = GetComponentInChildren<Button>(true);
        if (dimmerButton == null)
        {
            var dimmer = transform.Find("Dimmer");
            if (dimmer) dimmerButton = dimmer.GetComponent<Button>();
        }
    }

    public override void OpenUI()
    {
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);

        gameObject.SetActive(true);
        EnsureRefs(); // UIBase의 EnsureRefs 호출

        IsOpen = true;

        // CanvasGroup 설정은 OnOpen()에서 처리되도록 유지하거나, 여기서 즉시 설정
        if (canvasGroup)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        // 자체 OnOpen 훅 호출
        OnOpen();

        // 1. ContentRoot 숨김
        if (contentRoot != null) contentRoot.SetActive(false);

        // 2. 스프라이트 애니메이션 시작
        if (animationImage != null && animationFrames != null && animationFrames.Length > 0)
        {
            if (_animationCo != null) StopCoroutine(_animationCo);
            _animationCo = StartCoroutine(PlayOpenAnimationCoroutine());
        }
    }

    public override void CloseUI()
    {
        if (!gameObject.activeInHierarchy) return; // 이미 닫혀 있으면 무시

        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);
        EnsureRefs();

        // 닫기 명령이 들어오면 즉시 상호작용성 해제
        if (canvasGroup)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // 기존 코루틴(열림 애니메이션)이 있다면 정지하고 닫기 애니메이션 시작
        if (_animationCo != null) StopCoroutine(_animationCo);
        _animationCo = StartCoroutine(PlayCloseAnimationCoroutine()); // 닫기 애니메이션 코루틴 시작
    }


    protected override void OnOpen()
    {
        if (!Initialized)
        {
            if (closeButton) closeButton.onClick.AddListener(RequestClose);
            if (dimmerButton) dimmerButton.onClick.AddListener(RequestClose);
            Initialized = true;
        }
    }

    public void RequestClose()
    {
        if (!gameObject.activeInHierarchy) return;

        // 💡 오버라이드된 CloseUI()를 호출하여 닫기 애니메이션 시작
        CloseUI();
    }

    private IEnumerator CloseSequence()
    {
        yield return null;
    }

    protected override void OnClose()
    {
        // 닫기 명령 완료 시 애니메이션 코루틴 정리
        if (_animationCo != null)
        {
            StopCoroutine(_animationCo);
            _animationCo = null;
        }

        if (animationImage != null)
        {
            animationImage.enabled = false;
        }

        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator PlayOpenAnimationCoroutine()
    {
        animationImage.enabled = true; // Image 컴포넌트를 보이게 설정

        if (contentRoot != null) contentRoot.SetActive(false);

        for (int i = 0; i < animationFrames.Length; i++)
        {
            // 현재 프레임의 스프라이트를 Image 컴포넌트에 할당
            animationImage.sprite = animationFrames[i];

            // 마지막 프레임이 아닐 때만 대기
            if (i < animationFrames.Length - 1)
            {
                yield return new WaitForSecondsRealtime(frameDuration);
            }
        }

        // 애니메이션 완료 후 ContentRoot 활성화
        if (contentRoot != null) contentRoot.SetActive(true);

        _animationCo = null; // 코루틴 종료 후 참조 해제
    }

    private IEnumerator PlayCloseAnimationCoroutine()
    {
        // 닫기 애니메이션 시작 시 ContentRoot 비활성화
        if (contentRoot != null) contentRoot.SetActive(false);

        // 역순으로 반복
        for (int i = animationFrames.Length - 1; i >= 0; i--)
        {
            animationImage.sprite = animationFrames[i];

            // 0번 프레임이 아닐 때만 대기
            if (i > 0)
            {
                yield return new WaitForSecondsRealtime(frameDuration);
            }
        }

        // 닫는 애니메이션 완료 후, UIManager 비활성화 명령 호출
        _animationCo = null; // 코루틴 종료 후 참조 해제

        // OnClose 훅 호출 및 최종 비활성화
        OnClose();
        IsOpen = false;
        gameObject.SetActive(false);
    }
}