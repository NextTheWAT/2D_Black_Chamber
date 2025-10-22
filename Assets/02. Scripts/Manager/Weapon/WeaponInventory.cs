using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WeaponInventory : Singleton<WeaponInventory>
{
    public WeaponCatalog catalog;

    [SerializeField] private List<GunData> owned = new();
    public IReadOnlyList<GunData> Owned => owned;

    public UnityEvent OnInventoryChanged = new();
    public UnityEvent<GunData> OnItemBought = new();

    public bool IsOwned(GunData d) => d && owned.Contains(d);

    // ---- 내부용: 소유만 추가(스폰X) ----
    private bool AddOwnedSilently(GunData d)
    {
        if (!d || owned.Contains(d)) return false;
        owned.Add(d);
        return true;
    }

    // ---- 시작 시 1회: 기본 무기/기본 소유 동기화 ----
    private void SyncOwnedOnStart()
    {
        // 1) WeaponManager 초기 로드아웃에 있는 무기는 소유로 간주
        var wm = WeaponManager.Instance;
        if (wm && wm.intializeDatas != null)
        {
            foreach (var d in wm.intializeDatas)
                AddOwnedSilently(d);
        }

        // 2) 카탈로그에서 autoOwnedOnStart 체크된 무기 추가
        if (catalog)
        {
            foreach (var d in catalog.All.Where(x => x && x.autoOwnedOnStart))
                AddOwnedSilently(d);
        }

        OnInventoryChanged.Invoke();
    }

    private void Start()
    {
        // 씬 시작 시 한 번만 동기화
        SyncOwnedOnStart();
    }

    // ---- 상점: 구매 → 소유 추가 → WeaponManager에 스폰/장착 ----
    public bool Buy(GunData data, bool autoEquip = true)
    {
        if (!data || IsOwned(data)) return false;
        if (!MoneyManager.Instance.TrySpend(data.price)) return false;

        // 소유 추가
        owned.Add(data);
        OnInventoryChanged.Invoke();

        // 인게임 무기 생성/등록(+옵션: 자동장착)
        WeaponManager.Instance.AddWeapon(data, autoEquip);

        OnItemBought.Invoke(data);
        return true;
    }

    // ---- 상점 리스트 헬퍼 ----
    public IEnumerable<GunData> GetShopList(bool showOwned = false, bool hideHidden = true)
    {
        var all = catalog ? catalog.All : Enumerable.Empty<GunData>();
        if (hideHidden) all = all.Where(d => !d.hideFromShop);
        return showOwned ? all : all.Where(d => !IsOwned(d));
    }

    public ILookup<string, GunData> GetShopGroups(bool showOwned = false, bool hideHidden = true)
    {
        return GetShopList(showOwned, hideHidden)
            .ToLookup(d => d != null ? d.phaseTag.ToString() : "General"); // "Any", "Stealth", "Combat"
    }
}
