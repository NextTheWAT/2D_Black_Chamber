using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gunimage : MonoBehaviour
{
    [Header("UI on Slot")]
    public Image iconImage;      // 썸네일(현재 무기 대표 아이콘)
    public Image coverImage;     // 등급 커버 색
    public Image borderImage;    // 등급 테두리 색
    public TMP_Text nameText;    // 무기 이름

    [SerializeField] private ArmorySlotViewSimple armorySlotView;

    // 슬라이드/패널 제거 버전이므로 의미 없는 더미 값(호환용)
    public int CurrentIndex => 0;
    public float SlideDuration => 0f;

    private GunData currentData;



    // ================== 공개 API ==================
    // 데이터 통째로 세팅(메인 이미지 + 오버레이 갱신)
    public void SetData(GunData data)
    {
        currentData = data;
        ApplyOverlay(data);
    }


    public void SlideWithSprites(bool toRight, Sprite fromSprite, Sprite toSprite, System.Action onComplete = null)
    {
        //SetMainSprite(toSprite);
        onComplete?.Invoke();
    }

    // 오버레이(아이콘/커버/보더/이름) 갱신만 하고 싶을 때
    public void ApplyOverlay(GunData gundata)
    {
        if (gundata == null) return;

        // 등급 색 인덱스 계산
        int gradeIndex = Mathf.Clamp(gundata.Grade, 0,
            (armorySlotView && armorySlotView.backgroundGradeColors != null)
                ? armorySlotView.backgroundGradeColors.Length - 1 : 0);

        // 아이콘
        if (iconImage)
            iconImage.sprite = gundata.prefabInfo ? gundata.prefabInfo.weaponSprite : null;

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

    // 이전 SetAllSprites 이름 유지(호출부 호환): 오버레이 + 메인 스프라이트 동시 갱신
    public void SetAllSprites(Sprite sprite, GunData gundata)
    {
        if (gundata != null) ApplyOverlay(gundata);
    }

}
