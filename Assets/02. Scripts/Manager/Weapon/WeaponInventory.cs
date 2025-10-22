using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 소유/구매만 담당. 시작 보유 + 카탈로그(autoOwnedOnStart)로 초기화.
/// 상점은 여기만 건드린다. (장착/탄약/페이즈는 절대 개입 X)
/// </summary>
public sealed class WeaponInventory : Singleton<WeaponInventory>
{
    [Header("Catalog (Optional)")]
    public WeaponCatalog catalog;

    [Header("Starter Owned")]
    [SerializeField] private List<GunData> starterOwned = new();

    [Header("Runtime Owned (ReadOnly)")]
    [SerializeField] private List<GunData> owned = new();
    public IReadOnlyList<GunData> Owned => owned;

    [Header("Events")]
    public UnityEvent OnInventoryChanged = new();
    public UnityEvent<GunData> OnItemBought = new(); // Manager가 구독해서 Shooter 1개만 생성/장착

    private void Start() => RebuildOwnedFromStarters();

    public void RebuildOwnedFromStarters()
    {
        owned.Clear();

        foreach (var d in starterOwned) if (d) AddOwnedSilently(d);
        if (catalog)
        {
            foreach (var d in catalog.All)
                if (d && d.autoOwnedOnStart) AddOwnedSilently(d);
        }
        OnInventoryChanged.Invoke();
    }

    // === Shop 전용 API ===
    public bool Buy(GunData data)
    {
        if (!data || owned.Contains(data)) return false;

        // TODO: 프로젝트에 돈 시스템 있으면 여기서 결제 처리
        // if (!MoneyManager.Instance.TrySpend(data.price)) return false;

        owned.Add(data);
        OnInventoryChanged.Invoke();
        OnItemBought.Invoke(data);
        return true;
    }

    public bool IsOwned(GunData data) => data && owned.Contains(data);

    // === 조회/헬퍼 ===
    public IEnumerable<GunData> GetShopList(bool includeOwned = false, bool hideHidden = true)
    {
        var all = catalog ? catalog.All : Enumerable.Empty<GunData>();
        if (hideHidden) all = all.Where(d => !d.hideFromShop);
        return includeOwned ? all : all.Where(d => !IsOwned(d));
    }

    public GunData FindById(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;

        // 우선 카탈로그 → 없으면 Owned에서 찾기
        IEnumerable<GunData> src = catalog ? catalog.All : owned;

        foreach (var d in src)
            if (d && (d.name == key || d.displayName == key))   // ← id 제거, name/표시명으로 매칭
                return d;

        return null;
    }

    private void AddOwnedSilently(GunData d)
    {
        if (d && !owned.Contains(d)) owned.Add(d);
    }
}
