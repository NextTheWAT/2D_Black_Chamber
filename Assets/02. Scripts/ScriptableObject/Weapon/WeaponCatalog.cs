using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Weapon Catalog", fileName = "WeaponCatalog")]
public class WeaponCatalog : ScriptableObject
{
    [Tooltip("상점/무기방 등에 노출할 GunData 목록")]
    public List<GunData> items = new List<GunData>();

    public IEnumerable<GunData> All => items.Where(d => d != null);

    public IEnumerable<GunData> FilterByTag(GunData.PhaseTag tag)
    {
        if (tag == GunData.PhaseTag.Any) return All;
        return All.Where(d => d.phaseTag == tag || d.phaseTag == GunData.PhaseTag.Any);
    }

    public GunData FindByName(string assetName)
        => All.FirstOrDefault(d => d && d.name == assetName);

    public GunData FindByDisplayName(string display)
        => All.FirstOrDefault(d => d && d.displayName == display);
}
