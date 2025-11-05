using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Refs (외부 주입)")]
    public HoverPopup hoverPopup;           // StoreCanvas 밑 HoverPopupPanel (1개)
    public PurchasePopup purchasePopup;     // StoreCanvas 밑 PurchasePanel (1개)

    [Header("UI on Slot")]
    public Button clickButton;             // 슬롯 버튼
    public Image iconImage;               // 썸네일
    public Image coverImage;             // 등급 커버 이미지
    public Image borderImage;           // 테두리 이미지
    public TMP_Text nameText;         // 아이템 이름 텍스트
    public TMP_Text priceText;       // 아이템 가격 텍스트

    [Header("Sprites")]
    public Sprite basicBackgroundSprite; // 기본 배경 스프라이트
    public Sprite basicBorderSprite;           // 기본 테두리 스프라이트
    public Sprite hoverBackgroundSprite; // 마우스 올렸을 때 스프라이트
    public Sprite hoverBorderSprite;     // 마우스 올렸을 때 테두리 스프라이트

    [Header("Popup Positioning")]
    public Vector2 offset = new Vector2(400f, 0f);

    private GunData gun;
    private bool isHovering = false;

    private void Awake()
    {
        if (!clickButton) clickButton = GetComponent<Button>();
        if (hoverPopup) hoverPopup.Hide();
    }

    /// <summary>컨트롤러에서 슬롯 하나 바인딩</summary>
    public void Bind(GunData gunData, HoverPopup popupHover, PurchasePopup popupPurchase, bool isRightColumn)
    {
        gun = gunData;
        hoverPopup = popupHover;
        purchasePopup = popupPurchase;

        UpdateNameText(gunData);
        UpdatePriceText(gunData);
        UpdateGradeImageColor(gunData);

        if (iconImage)
            iconImage.sprite = gunData?.prefabInfo.weaponSprite;

        // if (iconImage) iconImage.sprite = GetSprite(gun, "weaponSprite") ?? GetSprite(gun, "shopIcon");

        if (!clickButton) clickButton = GetComponent<Button>();
        if (clickButton)
        {
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(() =>
            {
                if (purchasePopup && gun != null) purchasePopup.Show(gun);
            });

            UpdateNameText(gunData);
            UpdatePriceText(gunData);
            UpdateGradeImageColor(gunData);
        }
    }

    private void UpdateNameText(GunData gunData)
    {
        if (gunData == null) return;

        if (nameText)
            nameText.text = gunData.weaponName;
    }

    private void UpdatePriceText(GunData gunData)
    {
        if (gunData == null) return;

        if (priceText)
            priceText.text = $"$ {gunData.price:N0}";
    }

    private void UpdateGradeImageColor(GunData gunData)
    {
        if (gunData == null) return;

        int gradeIndex = Mathf.Clamp(gunData.Grade, 0, hoverPopup.backgroundGradeColors.Length - 1);

        if(coverImage)
            coverImage.color = hoverPopup.backgroundGradeColors[gradeIndex];

        if(borderImage)
            borderImage.color = hoverPopup.borderGradeColors[gradeIndex];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovering || hoverPopup == null || gun == null) return;
        isHovering = true;
        coverImage.sprite = hoverBackgroundSprite;
        borderImage.sprite = hoverBorderSprite;
        StartCoroutine(ShowPopupDelay());
    }

    private IEnumerator ShowPopupDelay()
    {
        yield return null; // 한 프레임 지연(이벤트 충돌 방지)

        Vector3 pos = transform.position;
        pos.x += offset.x;
        pos.y += offset.y;

        hoverPopup.transform.position = pos;
        hoverPopup.Show(gun);  // GunData 직접 전달
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovering || hoverPopup == null) return;
        isHovering = false;
        coverImage.sprite = basicBackgroundSprite;
        borderImage.sprite = basicBorderSprite;
        hoverPopup.Hide();
    }

    // ---- util: 안전하게 Sprite 읽기(필드명 유연대응: weaponSprite 또는 shopIcon) ----
    private static Sprite GetSprite(object obj, string fieldName)
    {
        if (obj == null) return null;
        var f = obj.GetType().GetField(fieldName);
        return f != null ? f.GetValue(obj) as Sprite : null;
    }
}
