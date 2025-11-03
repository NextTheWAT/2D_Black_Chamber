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
        if (canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        if (closeButton == null) closeButton = GetComponentInChildren<Button>(true);
        if (dimmerButton == null)
        {
            var dimmer = transform.Find("Dimmer");
            if (dimmer) dimmerButton = dimmer.GetComponent<Button>();
        }
    }

    protected override void OnOpen()
    {
        if (!Initialized)
        {
            if (closeButton) closeButton.onClick.AddListener(RequestClose);
            if (dimmerButton) dimmerButton.onClick.AddListener(RequestClose);
            Initialized = true;
        }

        if (canvasGroup)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        // UI가 열릴 때 애니메이션 시작
        if (animationImage != null && animationFrames != null && animationFrames.Length > 0)
        {
            // 기존 코루틴이 있다면 정지하고 새로 시작
            if (_animationCo != null) StopCoroutine(_animationCo);
            _animationCo = StartCoroutine(PlayOpenAnimationCoroutine());
        }
    }

    public void RequestClose()
    {
        if (!gameObject.activeInHierarchy) return;

        // 닫기 명령이 들어오면 즉시 상호작용성 해제
        if (canvasGroup)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // 기존 코루틴(열림 애니메이션)이 있다면 정지하고 닫기 애니메이션 시작
        if (_animationCo != null) StopCoroutine(_animationCo);
        _animationCo = StartCoroutine(PlayCloseAnimationCoroutine());
    }

    private IEnumerator CloseSequence()
    {
        // 닫기 애니메이션 완료 후 UIManager에 비활성화를 요청
        yield return null;
        UIManager.Instance.CloseUI<SettingPopup>();
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

        if (contentRoot != null) contentRoot.SetActive(true);

        _animationCo = null; // 코루틴 종료 후 참조 해제
    }

    private IEnumerator PlayCloseAnimationCoroutine()
    {
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

        // 닫는 애니메이션 완료 후, UI 비활성화 명령 호출
        _animationCo = null; // 코루틴 종료 후 참조 해제
        StartCoroutine(CloseSequence());
    }
}