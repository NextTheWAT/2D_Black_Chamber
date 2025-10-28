using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Armory(무기방) 전용 컨트롤러.
/// - Owned를 기반으로 스텔스/난전 슬롯을 "선택 & 저장"만 수행.
/// - 구매/환불은 절대 여기서 처리하지 않는다.
/// </summary>
public class ArmoryController : MonoBehaviour
{
    public WeaponInventory Inventory => WeaponInventory.Instance;
    public LoadoutProfile Profile => LoadoutProfile.Instance;

    /// <summary>무기방 목록으로 노출할 후보들(= 현재 Owned)</summary>
    public IReadOnlyList<GunData> GetOwned() => Inventory ? Inventory.Owned : System.Array.Empty<GunData>();

    /// <summary>스텔스 슬롯에 무기 지정(저장)</summary>
    public void SelectStealth(GunData data)
    {
        if (!Profile || data == null) return;
        string key = data.weaponName;                  // ← id 대신 name 사용
        Profile.SetStealth(key, save: true);
    }

    public void SelectCombat(GunData data)
    {
        if (!Profile || data == null) return;
        string key = data.weaponName;                  // ← id 대신 name 사용
        Profile.SetCombat(key, save: true);
    }

    /// <summary>현재 저장된 로드아웃 id를 반환(무기방 UI 기본값 바인딩 용)</summary>
    public (string stealthId, string combatId) GetSavedIds()
    {
        if (!Profile) return ("", "");
        return (Profile.GetStealthId(), Profile.GetCombatId());
    }

    /// <summary>초기화(무기방 UI의 '초기화' 버튼)</summary>
    public void ResetLoadout() => Profile?.Clear(save: true);
}
