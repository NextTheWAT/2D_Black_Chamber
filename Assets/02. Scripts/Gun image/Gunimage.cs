using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Gunimage : MonoBehaviour
{
    [SerializeField] private float slideDistance = 300f;
    public RectTransform[] panels;
    public float slideDuration = 0.45f;

    private int currentIndex = 0;
    private bool isSliding = false;

    public int CurrentIndex => currentIndex;
    public float SlideDuration => slideDuration;

    private void Awake()
    {
        if (slideDistance <= 0f)
        {
            if (panels != null && panels.Length > 0 && panels[0])
                slideDistance = panels[0].rect.width > 0 ? panels[0].rect.width
                                                         : ((RectTransform)transform).rect.width;
            if (slideDistance <= 0f) slideDistance = 600f;
        }
        ResetPanels();
    }

    private void OnEnable() => ResetPanels();
    private void OnDisable()
    {
        if (panels != null)
            foreach (var rt in panels) if (rt) rt.DOKill();
        isSliding = false;
    }

    private void ResetPanels()
    {
        if (panels == null) return;
        for (int i = 0; i < panels.Length; i++)
        {
            var rt = panels[i];
            if (!rt) continue;
            var le = rt.GetComponent<LayoutElement>();
            if (le) le.ignoreLayout = true; // 레이아웃 간섭 방지
            rt.anchoredPosition = Vector2.zero;
            rt.gameObject.SetActive(i == currentIndex);
        }
        isSliding = false;
    }

    // Armory가 "들어올 패널"을 알 수 있게 제공
    public int PeekTargetIndex(bool plus)
    {
        int idx = currentIndex + (plus ? 1 : -1);
        if (idx >= panels.Length) idx = 0;
        else if (idx < 0) idx = panels.Length - 1;
        return idx;
    }
    public RectTransform GetPanel(int index) => (index >= 0 && index < panels.Length) ? panels[index] : null;

    public void clickshowpanel(bool plus)
    {
        ShowPanel(PeekTargetIndex(plus), plus);
    }

    private void ShowPanel(int index, bool plus)
    {
        if (isSliding || index == currentIndex || panels == null || panels.Length == 0) return;

        bool moveLeft = plus; // 오른쪽 버튼(plus=true)이면 왼쪽으로 미는 연출
        Vector2 offset = new Vector2(moveLeft ? -slideDistance : slideDistance, 0f);

        isSliding = true;

        RectTransform fromPanel = panels[currentIndex];
        RectTransform toPanel = panels[index];

        fromPanel.DOKill();
        toPanel.DOKill();

        fromPanel.transform.SetAsLastSibling();
        toPanel.transform.SetAsFirstSibling();

        toPanel.gameObject.SetActive(true);
        toPanel.anchoredPosition = -offset;

        // ★ 타임스케일 무시: SetUpdate(true)
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

    // (옵션) 외부에서 패널 스프라이트를 지정할 때 사용
    public void SetPanelSprite(int panelIndex, Sprite sprite)
    {
        var p = GetPanel(panelIndex);
        if (!p) return;
        var img = p.GetComponent<Image>();
        if (img) img.sprite = sprite;
    }
}
