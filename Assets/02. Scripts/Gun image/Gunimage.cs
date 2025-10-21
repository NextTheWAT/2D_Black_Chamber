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
    // 잠시

    public RectTransform[] panels; // 전환 대상 패널들
    public float slideDuration = 0.5f; // 슬라이드 애니메이션 지속 시간

    private bool isSliding = false; // 슬라이딩 중 여부 체크

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    void Awake()
    {
        image = GetComponent<Image>(); 
        //image.sprite = sprites[currentIndex]; // 시작 시 첫 번째 이미지 표시
    }

    //public void OnLeftButton()
    //{
    //    currentIndex--; //왼쪽으로 이동

    //    // 인덱스가 0보다 작아지면 맨 끝으로
    //    if (currentIndex < 0)
    //    {
    //        currentIndex = sprites.Length - 1;
    //    }

    //    image.sprite = sprites[currentIndex];
    //}
    //public void OnRightButton()
    //{
    //    currentIndex++; //오른쪽으로 이동

    //    // 마지막 이미지 이후면 맨 앞으로
    //    if (currentIndex >= sprites.Length)
    //    {
    //        currentIndex = 0;
    //    }

    //    image.sprite = sprites[currentIndex];
    //}


    public void ShowPanel(int index)
    {
        // 슬라이딩 중이거나 같은 패널이면 무시
        if (index == currentIndex || isSliding) return;

        // 인덱스 범위 벗어나면 순환 처리
        if (index >= panels.Length) index = 0;
        else if (index < 0) index = panels.Length - 1;

        isSliding = true;

        RectTransform fromPanel = panels[currentIndex];
        RectTransform toPanel = panels[index];

        // 패널 순서 조정 (새 패널을 뒤에 배치)
        fromPanel.transform.SetAsLastSibling();
        toPanel.transform.SetAsFirstSibling();

        // 이동 방향 계산 (왼쪽/오른쪽)
        Vector2 panelPosition = currentIndex > index ? new Vector2(Screen.width, 0) : new Vector2(-Screen.width, 0);

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
                fromPanel.anchoredPosition = Vector2.zero;
                currentIndex = index;
                isSliding = false;
            });
    }

}
