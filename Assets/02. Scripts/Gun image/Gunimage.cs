using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gunimage : MonoBehaviour
{
    [SerializeField] private float slideDistance = 300f;
    public RectTransform[] panels;
    public float slideDuration = 0.45f;

    private int currentIndex = 0;
    private bool isSliding = false;

    [Header("UI on Slot")]
    public Image iconImage;               // 썸네일(현재 무기 대표 아이콘)
    public Image coverImage;              // 등급 커버 색
    public Image borderImage;             // 등급 테두리 색
    public TMP_Text nameText;             // 무기 이름

    [SerializeField] private ArmorySlotViewSimple armorySlotView;

    public int CurrentIndex => currentIndex;
    public float SlideDuration => slideDuration;

    // =============== Unity lifecycle ===============
    private void Awake()
    {
        // panels가 있고 거리 미지정이면 패널 폭 기준 자동 산정
        if ((slideDistance <= 0f) && panels != null && panels.Length > 0 && panels[0])
        {
            // 첫 패널(혹은 컨테이너)의 너비 기반 슬라이드 거리
            var w = panels[0].rect.width;
            if (w <= 0f && transform is RectTransform rt) w = rt.rect.width;
            slideDistance = (w > 0f) ? w : 600f;
        }
        ResetPanels();
    }

    private void OnEnable() => ResetPanels();

    private void OnDisable()
    {
        if (panels != null)
        {
            foreach (var rt in panels)
            {
                if (rt) rt.DOKill();
            }
        }
        isSliding = false;
    }

    // 초기 패널 상태 정리
    private void ResetPanels()
    {
        if (panels == null || panels.Length == 0) return;

        // currentIndex 안전 보정
        if (currentIndex < 0 || currentIndex >= panels.Length) currentIndex = 0;

        for (int i = 0; i < panels.Length; i++)
        {
            var rt = panels[i];
            if (!rt) continue;

            // 레이아웃 간섭 방지
            var le = rt.GetComponent<LayoutElement>();
            if (le) le.ignoreLayout = true;

            rt.anchoredPosition = Vector2.zero;
            rt.gameObject.SetActive(i == currentIndex);
        }
        isSliding = false;
    }

    // 외부에서 "다음/이전 패널 인덱스" 미리 확인
    public int PeekTargetIndex(bool plus)
    {
        int idx = currentIndex + (plus ? 1 : -1);
        if (idx >= panels.Length) idx = 0;
        else if (idx < 0) idx = panels.Length - 1;
        return idx;
    }

    public RectTransform GetPanel(int index)
        => (index >= 0 && index < panels.Length) ? panels[index] : null;

    public void clickshowpanel(bool plus)
    {
        ShowPanel(PeekTargetIndex(plus), plus);
    }

    // 단순 패널 슬라이드(스프라이트 교체 없이 위치만 이동)
    private void ShowPanel(int index, bool plus)
    {
        if (isSliding || panels == null || panels.Length == 0) return;
        if (index == currentIndex) return;

        bool moveLeft = plus; // 오른쪽 버튼(plus=true)이면 왼쪽으로 미는 연출
        Vector2 offset = new Vector2(moveLeft ? -slideDistance : slideDistance, 0f);

        isSliding = true;

        var fromPanel = panels[currentIndex];
        var toPanel = panels[index];
        if (!fromPanel || !toPanel) { isSliding = false; return; }

        // 정렬
        fromPanel.transform.SetAsLastSibling();
        toPanel.transform.SetAsFirstSibling();

        toPanel.gameObject.SetActive(true);
        toPanel.anchoredPosition = -offset;

        // 타임스케일 무시: SetUpdate(true)
        var t1 = fromPanel.DOAnchorPos(offset, slideDuration).SetEase(Ease.OutCubic).SetUpdate(true);
        var t2 = toPanel.DOAnchorPos(Vector2.zero, slideDuration).SetEase(Ease.OutCubic).SetUpdate(true);

        t2.OnComplete(() =>
        {
            fromPanel.anchoredPosition = Vector2.zero; // 누적 오프셋 제거
            fromPanel.gameObject.SetActive(false);
            currentIndex = index;
            isSliding = false;
        });
    }

    // =============== 슬라이드(스프라이트 포함) ===============
    // 기존 호출 호환: 슬라이드 전 from/to 패널의 이미지 스프라이트를 바꿔놓고 이동
    public void SlideWithSprites(bool toRight, Sprite fromSprite, Sprite toSprite, System.Action onComplete = null)
    {
        if (panels == null || panels.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }

        // 들어올 패널 인덱스 계산
        int toIndex = currentIndex + (toRight ? 1 : -1);
        if (toIndex >= panels.Length) toIndex = 0;
        else if (toIndex < 0) toIndex = panels.Length - 1;

        bool moveLeft = toRight; // 오른쪽 버튼이면 왼쪽으로 미는 연출
        Vector2 offset = new Vector2(moveLeft ? -slideDistance : slideDistance, 0f);

        var fromPanel = panels[currentIndex];
        var toPanel = panels[toIndex];
        if (!fromPanel || !toPanel)
        {
            onComplete?.Invoke();
            return;
        }

        // from/to 패널의 Image에 각각 현재/다음 스프라이트 적용(슬라이드 직전)
        var fromImg = fromPanel.GetComponent<Image>();
        var toImg = toPanel.GetComponent<Image>();
        if (fromImg) fromImg.sprite = fromSprite;
        if (toImg) toImg.sprite = toSprite;

        fromPanel.transform.SetAsLastSibling();
        toPanel.transform.SetAsFirstSibling();

        toPanel.gameObject.SetActive(true);
        toPanel.anchoredPosition = -offset;

        var t1 = fromPanel.DOAnchorPos(offset, slideDuration).SetEase(Ease.OutCubic).SetUpdate(true);
        var t2 = toPanel.DOAnchorPos(Vector2.zero, slideDuration).SetEase(Ease.OutCubic).SetUpdate(true);

        t2.OnComplete(() =>
        {
            // fromPanel 원상복귀 및 비활성
            fromPanel.anchoredPosition = Vector2.zero;
            fromPanel.gameObject.SetActive(false);

            currentIndex = toIndex;
            onComplete?.Invoke();
        });
    }

    // 신규: GunData까지 받아서 슬라이드 완료 시 오버레이 & 패널 일괄 갱신
    public void SlideWithData(bool toRight, GunData fromData, GunData toData, System.Action onComplete = null)
    {
        // 안전한 널 체이닝으로 스프라이트 뽑기
        Sprite fromSprite = fromData?.prefabInfo?.weaponSprite;
        Sprite toSprite = toData?.prefabInfo?.weaponSprite;

        SlideWithSprites(toRight, fromSprite, toSprite, () =>
        {
            // 슬라이드 완료 시 오버레이 + 패널 일괄 갱신
            SetAllSprites(toSprite, toData);
            onComplete?.Invoke();
        });
    }

    // =============== 오버레이 & 패널 갱신 ===============
    // 오버레이(아이콘/커버/보더/네임)만 갱신하고 싶을 때
    public void ApplyOverlay(GunData gundata)
    {
        if (gundata == null) return;

        // 등급 색 인덱스
        int gradeIndex = Mathf.Clamp(gundata.Grade, 0,
            (armorySlotView && armorySlotView.backgroundGradeColors != null)
            ? armorySlotView.backgroundGradeColors.Length - 1 : 0);

        // 아이콘
        if (iconImage)
            iconImage.sprite = (gundata.prefabInfo) ? gundata.prefabInfo.weaponSprite : null;

        // 커버/보더 색
        if (coverImage && armorySlotView && armorySlotView.backgroundGradeColors != null
            && armorySlotView.backgroundGradeColors.Length > 0)
        {
            gradeIndex = Mathf.Clamp(gradeIndex, 0, armorySlotView.backgroundGradeColors.Length - 1);
            coverImage.color = armorySlotView.backgroundGradeColors[gradeIndex];
        }

        if (borderImage && armorySlotView && armorySlotView.borderGradeColors != null
            && armorySlotView.borderGradeColors.Length > 0)
        {
            gradeIndex = Mathf.Clamp(gradeIndex, 0, armorySlotView.borderGradeColors.Length - 1);
            borderImage.color = armorySlotView.borderGradeColors[gradeIndex];
        }

        // 이름
        if (nameText) nameText.text = gundata.weaponName;
    }

    // 패널 전체 스프라이트 + 오버레이를 한 번에 갱신
    public void SetAllSprites(Sprite sprite, GunData gundata)
    {
        // 먼저 오버레이 적용(아이콘/색/이름)
        if (gundata != null) ApplyOverlay(gundata);

        if (panels == null) return;

        foreach (var rt in panels)
        {
            if (!rt) continue;
            var img = rt.GetComponent<Image>();
            if (!img) continue;

            var r = img.rectTransform;
            r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
            r.anchoredPosition = Vector2.zero;

            img.preserveAspect = true;
            img.sprite = sprite;
        }
    }
}
