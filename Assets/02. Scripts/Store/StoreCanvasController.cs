using System.Linq;
using TMPro;
using UnityEngine;

public class StoreCanvasController : MonoBehaviour
{
    [Header("Refs")]
    public Transform slotsParent;       // WeaponsPanel/Slots
    public HoverPopup hoverPopup;       // HoverPopupPanel (비활성 시작)
    public PurchasePopup purchasePopup; // PurchasePopup 컴포넌트(1개)
    public WeaponCatalog catalog;       // 카탈로그 에셋
    public TMP_Text moneyText;          // 잔액 표시(선택)

    [Header("Options")]
    public bool showOwnedInStore = false;  // 이미 소유도 표시?
    public bool hideHiddenInStore = true;  // hideFromShop 숨김?

    private ItemSlot[] slots;

    private void Awake()
    {
        slots = slotsParent.GetComponentsInChildren<ItemSlot>(includeInactive: true);
    }

    private void OnEnable()
    {
        MoneyManager.Instance.OnMoneyChanged.AddListener(RefreshMoney);
        WeaponInventory.Instance.OnInventoryChanged.AddListener(RefreshGrid);

        if (FirebaseManager.Instance.IsGunDataLoaded)
        {
            RefreshMoney();
            RefreshGrid();
        }
        else
        {
            FirebaseManager.Instance.GunDataLoaded += RefreshMoney;
            FirebaseManager.Instance.GunDataLoaded += RefreshGrid;
        }
    }

    private void OnDisable()
    {
        if (MoneyManager.Instance) MoneyManager.Instance.OnMoneyChanged.RemoveListener(RefreshMoney);
        if (WeaponInventory.Instance) WeaponInventory.Instance.OnInventoryChanged.RemoveListener(RefreshGrid);
        if (FirebaseManager.Instance)
        {
            FirebaseManager.Instance.GunDataLoaded -= RefreshMoney;
            FirebaseManager.Instance.GunDataLoaded -= RefreshGrid;
        }
    }

    private void RefreshMoney()
    {
        if (moneyText) moneyText.text = $"$ {MoneyManager.Instance.Balance:N0}";
    }

    public void RefreshGrid()
    {
        var list = WeaponInventory.Instance
            .GetShopList(showOwnedInStore, hideHiddenInStore)
            .Take(slots.Length)
            .ToList();

        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            if (!slot) continue;

            if (i < list.Count)
            {
                var gun = list[i];
                bool isRightColumn = i >= 4; // 4x2 레이아웃 기준

                // 새 인터페이스 한 줄로 바인딩
                slot.Bind(gun, hoverPopup, purchasePopup, isRightColumn);
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }
}
