using UnityEngine;
using UnityEngine.Events;
using Constants;

/// <summary>
/// Armory(무기방) 도메인의 저장소.
/// - 소유(Owned)와 무관하게, "스텔스/난전" 로드아웃 선택 결과만 저장한다.
/// - PlayerPrefs로 간단 저장/로드.
/// - WeaponManager는 이 값을 "읽기"만 한다.
/// </summary>
public sealed class LoadoutProfile : Singleton<LoadoutProfile>
{
    [Header("Saved Gun Ids (string)")]
    [SerializeField] private string stealthGunId = "";  // 예: "pistol_01"
    [SerializeField] private string combatGunId = "";  // 예: "rifle_01"

    [System.Serializable]
    public class LoadoutChangedEvent : UnityEvent<GamePhase, string> { }
    public LoadoutChangedEvent OnLoadoutChanged = new();

    const string KEY_STEALTH = "LOADOUT_STEALTH_ID";
    const string KEY_COMBAT = "LOADOUT_COMBAT_ID";

    private void Awake()
    {
        // PlayerPrefs에서 복원
        stealthGunId = PlayerPrefs.GetString(KEY_STEALTH, stealthGunId);
        combatGunId = PlayerPrefs.GetString(KEY_COMBAT, combatGunId);
    }

    /// <summary>스텔스 슬롯의 무기 id를 설정하고 저장</summary>
    public void SetStealth(string gunId, bool save = true)
    {
        if (string.IsNullOrEmpty(gunId) || gunId == stealthGunId) return;
        stealthGunId = gunId;
        if (save) PlayerPrefs.SetString(KEY_STEALTH, stealthGunId);
        OnLoadoutChanged.Invoke(GamePhase.Stealth, gunId);
    }

    /// <summary>난전 슬롯의 무기 id를 설정하고 저장</summary>
    public void SetCombat(string gunId, bool save = true)
    {
        if (string.IsNullOrEmpty(gunId) || gunId == combatGunId) return;
        combatGunId = gunId;
        if (save) PlayerPrefs.SetString(KEY_COMBAT, combatGunId);
        OnLoadoutChanged.Invoke(GamePhase.Combat, gunId);
    }

    public string GetStealthId() => stealthGunId;
    public string GetCombatId() => combatGunId;

    /// <summary>로드아웃 전체 초기화(무기방에서 '초기화' 버튼 용)</summary>
    public void Clear(bool save = true)
    {
        stealthGunId = combatGunId = "";
        if (save)
        {
            PlayerPrefs.DeleteKey(KEY_STEALTH);
            PlayerPrefs.DeleteKey(KEY_COMBAT);
        }
        OnLoadoutChanged.Invoke(GamePhase.Stealth, stealthGunId);
        OnLoadoutChanged.Invoke(GamePhase.Combat, combatGunId);
    }
}
