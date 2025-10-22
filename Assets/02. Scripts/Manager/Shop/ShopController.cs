using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상점 전용 컨트롤러(UI에서 이걸 붙잡고 쓴다)
/// - 판매 목록 노출
/// - 구매(소유 변경)만 처리
/// - 장착/로드아웃 변화는 절대 처리하지 않는다.
/// </summary>
public class ShopController : MonoBehaviour
{
    public WeaponInventory Inventory => WeaponInventory.Instance;

    public IEnumerable<GunData> GetForSale(bool includeOwned = false, bool hideHidden = true)
        => Inventory ? Inventory.GetShopList(includeOwned, hideHidden) : System.Array.Empty<GunData>();

    public bool TryBuy(GunData data)
    {
        if (!Inventory) return false;
        return Inventory.Buy(data);  // 여기서 끝(장착 변경 X)
    }
}
