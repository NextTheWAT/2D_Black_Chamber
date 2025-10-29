using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchasePopup : MonoBehaviour
{
    [Header("Panels")]
    public GameObject purchasePanel;

    [Header("UI")]
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Button buyButton;
    public Button closeButton;

    [Header("External")]
    public PurchaseCondition condition;

    private GunData pending;

    private void Awake()
    {
        if (buyButton) buyButton.onClick.AddListener(BuyNow);
        if (closeButton) closeButton.onClick.AddListener(ClosePurchasePopup);
        ClosePurchasePopup();
    }

    public void Show(GunData data)
    {
        pending = data;
        var nameToShow = WeaponCatalog.GetDisplayName(data);
        if (itemNameText) itemNameText.text = data != null ? nameToShow : "-";
        if (priceText) priceText.text = data != null ? $"$ {data.price:N0}" : "$ 0";
        ShowPurchasePopup();
    }

    public void ShowPurchasePopup()
    {
        if (purchasePanel) purchasePanel.SetActive(true);
    }

    public void ClosePurchasePopup()
    {
        if (purchasePanel) purchasePanel.SetActive(false);
    }

    private void BuyNow()
    {
        if (pending == null) return;
        bool ok = WeaponInventory.Instance.Buy(pending);
        ClosePurchasePopup();
        if (condition != null) condition.EnoughMoneyPopup(ok);
    }
}
