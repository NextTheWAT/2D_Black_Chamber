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
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovering || hoverPopup == null || gun == null) return;
        isHovering = true;
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
