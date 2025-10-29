using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;

public sealed class WeaponInventory : Singleton<WeaponInventory>
{
    [Header("Catalog (Optional)")]
    public WeaponCatalog catalog;

    [Header("Starter Owned")]
    [SerializeField] private List<string> starterOwnedId = new();
    // [SerializeField] private List<GunData> starterOwned = new();

    [Header("Runtime Owned (ReadOnly)")]
    [SerializeField] private List<GunData> owned = new();
    public IReadOnlyList<GunData> Owned => owned;

    [Header("Events")]
    public UnityEvent OnInventoryChanged = new();
    public UnityEvent<GunData> OnItemBought = new();

    private void OnEnable()
    {
        if (FirebaseManager.Instance.IsGunDataLoaded)
            RebuildOwnedFromStarters();
        else
            FirebaseManager.Instance.GunDataLoaded += RebuildOwnedFromStarters;
    }
    void OnDisable()
    {
        if (GameManager.AppIsQuitting) return;
        FirebaseManager.Instance.GunDataLoaded -= RebuildOwnedFromStarters;
    }


    public void RebuildOwnedFromStarters()
    {
        catalog.Initialize();

        owned.Clear();

        foreach (var id in starterOwnedId)
        {
            GunData gunData = FirebaseManager.Instance.GetGunData(id);
            if (gunData != null)
                AddOwnedSilently(gunData);
        }

        // foreach (var d in starterOwned) if (d != null) AddOwnedSilently(d);

        if (catalog)
        {
            foreach (var d in catalog.All)
                if (d != null && d.prefabInfo.autoOwnedOnStart) AddOwnedSilently(d);
        }
        OnInventoryChanged.Invoke();
    }

    public bool Buy(GunData data)
    {
        if (data == null || owned.Contains(data)) return false;

        // 결제 시도: 잔액 부족이면 실패 처리
        var money = MoneyManager.Instance;
        if (money == null)
        {
            Debug.LogError("[WeaponInventory] MoneyManager가 씬에 없습니다.");
            return false;
        }
        if (!money.TrySpend(data.price))   // ← 여기서 차감 + OnChanged 이벤트 발행
            return false;

        owned.Add(data);
        OnInventoryChanged.Invoke();
        OnItemBought.Invoke(data);
        return true;
    }

    public bool IsOwned(GunData data) => data != null && owned.Contains(data);

    public IEnumerable<GunData> GetShopList(bool includeOwned = false, bool hideHidden = true)
    {
        var all = catalog ? catalog.All : Enumerable.Empty<GunData>();
        foreach (var item in all)
        {
            Debug.Log(item.weaponName);
        }

        if (hideHidden)
        {
            all = all.Where(d => d.prefabInfo? !d.prefabInfo.hideFromShop : false);
        }
        return includeOwned ? all : all.Where(d => !IsOwned(d));
    }

    /// <summary>name / weaponName / displayName / id(int)로 유연 검색</summary>
    public GunData FindById(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        IEnumerable<GunData> src = catalog ? catalog.All : owned;

        foreach (var d in src)
        {
            if (d == null) continue;
            if (d.weaponName == key) return d;

            var weaponName = GetStringField(d, "weaponName");
            if (!string.IsNullOrEmpty(weaponName) && weaponName == key) return d;

            var displayName = GetStringField(d, "displayName"); // 구스펙 호환
            if (!string.IsNullOrEmpty(displayName) && displayName == key) return d;

            var id = GetIntField(d, "id");
            if (id.HasValue && id.Value.ToString() == key) return d;
        }
        return null;
    }

    private void AddOwnedSilently(GunData d)
    {
        if (d != null && !owned.Contains(d)) owned.Add(d);
    }

    // ---- reflection helpers (신/구 스펙 동시 호환) ----
    private static string GetStringField(object obj, string field)
    {
        var f = obj.GetType().GetField(field, BindingFlags.Instance | BindingFlags.Public);
        return f != null ? f.GetValue(obj) as string : null;
    }
    private static int? GetIntField(object obj, string field)
    {
        var f = obj.GetType().GetField(field, BindingFlags.Instance | BindingFlags.Public);
        if (f == null) return null;
        object v = f.GetValue(obj);
        return v is int iv ? iv : (int?)null;
    }
}
