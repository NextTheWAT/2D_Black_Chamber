using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gunimage : MonoBehaviour
{
    //public Sprite[] sprites;
    private Image image;
    private int currentIndex = 0;

    [SerializeField]
    private float slideDistance = 0f; // 원하는 슬라이드 거리 (픽셀)

    public RectTransform[] panels; // 전환 대상 패널들
    public float slideDuration = 0.5f; // 슬라이드 애니메이션 지속 시간

    private bool isSliding = false; // 슬라이딩 중 여부 체크

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;


    // 현재 보이는 패널 인덱스가 필요할 때 (읽기 전용)
    public int CurrentPanelIndex => currentIndex;

    public void clickshowpanel(bool plus)  //클릭 시 계속 넘어감
    {
        int targetindex = currentIndex;
        if (plus) targetindex++;
        else targetindex--;
        ShowPanel(targetindex);
    }
    private void ShowPanel(int index)
    {
        // 슬라이딩 중이거나 같은 패널이면 무시
        if (index == currentIndex || isSliding) return;

        // 이동 방향 계산 (왼쪽/오른쪽)
        Vector2 panelPosition = currentIndex > index ? new Vector2(slideDistance, 0) : new Vector2(-slideDistance, 0);

        // 인덱스 범위 벗어나면 순환 처리
        if (index >= panels.Length) index = 0;
        else if (index < 0) index = panels.Length - 1;

        isSliding = true;

        RectTransform fromPanel = panels[currentIndex];
        RectTransform toPanel = panels[index];

        // 패널 순서 조정 (새 패널을 뒤에 배치)
        fromPanel.transform.SetAsLastSibling();
        toPanel.transform.SetAsFirstSibling();

       

        // 새 패널 위치 지정 및 활성화
        toPanel.gameObject.SetActive(true);
        toPanel.anchoredPosition = panelPosition;

        // DOTween 애니메이션
        fromPanel.DOAnchorPos(-panelPosition, slideDuration).SetEase(Ease.OutCubic);

        toPanel.DOAnchorPos(Vector2.zero, slideDuration).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // 이전 패널 비활성화 및 리셋
                fromPanel.gameObject.SetActive(false);
                //fromPanel.anchoredPosition = Vector2.zero;
                currentIndex = index;
                isSliding = false;
            });
    }

    // 다음/이전 슬라이드 시 도착할 패널 인덱스 미리 보기
    public int PeekNextPanelIndex(bool plus)
    {
        int idx = currentIndex + (plus ? 1 : -1);
        if (idx >= panels.Length) idx = 0;
        else if (idx < 0) idx = panels.Length - 1;
        return idx;
    }

    // 특정 패널의 Image 스프라이트를 교체
    public void SetPanelSprite(int panelIndex, Sprite sprite)
    {
        if (panelIndex < 0 || panelIndex >= panels.Length) return;
        var img = panels[panelIndex].GetComponent<Image>();
        if (img) img.sprite = sprite;
    }

}
