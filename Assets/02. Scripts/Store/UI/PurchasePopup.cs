using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchasePopup : MonoBehaviour
{
    [Header("Panels")]
    public GameObject purchasePanel;      // 구매창 본체

    [Header("UI")]
    public TMP_Text itemNameText;         // 아이템 이름
    public TMP_Text priceText;            // 가격
    public Button buyButton;
    public Button closeButton;

    [Header("External")]
    public PurchaseCondition condition;   // 부족/완료 팝업 띄우는 헬퍼

    private GunData pending;              // 현재 선택한 GunData

    private void Awake()
    {
        if (buyButton) buyButton.onClick.AddListener(BuyNow);
        if (closeButton) closeButton.onClick.AddListener(ClosePurchasePopup);
        ClosePurchasePopup(); // 시작 시 닫아두기
    }

    /// <summary>슬롯에서 호출: 아이템 지정 후 구매창 오픈</summary>
    public void Show(GunData data)
    {
        pending = data;
        if (itemNameText) itemNameText.text = data ? data.displayName : "-";
        if (priceText) priceText.text = data ? $"$ {data.price:N0}" : "$ 0";

        ShowPurchasePopup();
    }

    // 기존 API 유지 (UI에서 연결되어 있을 수 있으니 그대로 둠)
    public void ShowPurchasePopup()
    {
        if (purchasePanel) purchasePanel.SetActive(true);
    }

    public void ClosePurchasePopup()
    {
        if (purchasePanel) purchasePanel.SetActive(false);
        // pending 은 유지/초기화 취향대로. 여기선 유지 안해도 OK
    }

    private void BuyNow()
    {
        if (!pending) return;

        // 인벤토리 구매만 수행(장착 변경 없음)
        bool ok = WeaponInventory.Instance.Buy(pending);   // ← autoEquip 파라미터 제거

        ClosePurchasePopup();

        if (condition != null)
            condition.EnoughMoneyPopup(ok);
    }
}
