using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

[CreateAssetMenu(menuName = "Game/Weapon Catalog", fileName = "WeaponCatalog")]
public class WeaponCatalog : ScriptableObject
{
    [Tooltip("상점/무기방 등에 노출할 GunData 목록")]
    public List<GunData> items = new List<GunData>();

    public IEnumerable<GunData> All => items.Where(d => d != null);

    public IEnumerable<GunData> FilterByTag(GunData.PhaseTag tag)
    {
        if (tag == GunData.PhaseTag.Any) return All;
        return All.Where(d => d.prefabInfo.phaseTag == tag || d.prefabInfo.phaseTag == GunData.PhaseTag.Any);
    }

    public GunData FindByName(string assetName)
        => All.FirstOrDefault(d => d != null && d.weaponName == assetName);

    public GunData FindByDisplayName(string display)
        => All.FirstOrDefault(d => d != null && GetDisplayName(d) == display);

    public static string GetDisplayName(GunData d)
    {
        if (d == null) return "";
        var t = d.GetType();
        var f1 = t.GetField("weaponName", BindingFlags.Instance | BindingFlags.Public);
        var f2 = t.GetField("displayName", BindingFlags.Instance | BindingFlags.Public); // 구스펙
        string val = f1?.GetValue(d) as string ?? f2?.GetValue(d) as string;
        return string.IsNullOrEmpty(val) ? d.weaponName : val;
    }
}
