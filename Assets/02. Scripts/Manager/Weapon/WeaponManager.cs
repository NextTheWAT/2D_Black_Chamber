using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Constants;

/// <summary>
/// 전투 런타임 전담:
/// - 씬 로드시 Owned 기반으로 Shooter 풀 생성
/// - LoadoutProfile 기준으로 스텔스/난전 슬롯 설정
/// - 페이즈 전환 시 자동 장착
/// - 탄약/HUD 이벤트, 전역 발사 잠금
/// (구매/환불/카탈로그/로드아웃 저장은 절대 관여 X)
/// </summary>
public sealed class WeaponManager : Singleton<WeaponManager>
{
    [Serializable] public class WeaponChangedEvent : UnityEvent<Shooter> { }
    public WeaponChangedEvent OnWeaponChanged = new();
    public UnityEvent OnAmmoChanged = new();   // Shooter에서 직접 호출하거나 여기서 Invoke
    public UnityEvent OnReloaded = new();      // 동일

    [Header("Fallback (Owned 비었을 때만)")]
    public GunData[] initializeDatas = Array.Empty<GunData>();
    public int initializeIndex = 0;

    [Header("Runtime (ReadOnly)")]
    [SerializeField] private List<Shooter> weaponSlots = new();
    [SerializeField] private int stealthSlotIndex = -1;
    [SerializeField] private int combatSlotIndex = -1;
    [SerializeField] private bool _globalShooterLocked = false;

    private int currentIndex = -1;
    public Shooter CurrentWeapon => IsValidSlot(currentIndex) ? weaponSlots[currentIndex] : null;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        var gm = GameManager.Instance; if (gm) gm.OnPhaseChanged += OnPhaseChanged;

        // 로드아웃 변경을 들으면 즉시 반영(허브 씬에서도 사용 가능)
        var lp = LoadoutProfile.Instance; if (lp) lp.OnLoadoutChanged.AddListener(OnLoadoutChanged);
        var inv = WeaponInventory.Instance; if (inv) inv.OnItemBought.AddListener(OnItemBought);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        var gm = GameManager.Instance; if (gm) gm.OnPhaseChanged -= OnPhaseChanged;

        var lp = LoadoutProfile.Instance; if (lp) lp.OnLoadoutChanged.RemoveListener(OnLoadoutChanged);
        var inv = WeaponInventory.Instance; if (inv) inv.OnItemBought.RemoveListener(OnItemBought);
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m) => RebuildFromOwned();
    private void OnPhaseChanged(GamePhase phase) => ApplyPhaseWeapon(phase);
    private void OnLoadoutChanged(GamePhase phase, string gunId) => ApplyLoadoutFromProfile();

    // === 핵심 빌드 ===
    public void RebuildFromOwned()
    {
        // 기존 풀 제거
        for (int i = 0; i < weaponSlots.Count; i++)
            if (weaponSlots[i]) Destroy(weaponSlots[i].gameObject);
        weaponSlots.Clear();

        currentIndex = -1; // 새 빌드 전에 반드시 리셋

        // 1) Owned 우선
        var inv = WeaponInventory.Instance;
        var owned = inv ? inv.Owned : null;

        if (owned != null && owned.Count > 0)
        {
            foreach (var d in owned) if (d) weaponSlots.Add(CreateShooter(d));
        }
        else
        {
            // 2) 폴백
            foreach (var d in initializeDatas) if (d) weaponSlots.Add(CreateShooter(d));
        }

        // 로드아웃 적용(슬롯 인덱스 확정)
        ApplyLoadoutFromProfile();

        // 현재 페이즈 무기 장착
        var gm = GameManager.Instance;
        var phase = gm ? gm.CurrentPhase : GamePhase.Stealth;
        ApplyPhaseWeapon(phase);

        Debug.Log("[WeaponManager] Rebuild complete. Total shooters: " + weaponSlots.Count);

        Debug.Log($"idx={currentIndex}, stealth={stealthSlotIndex}, " +
          $"active={(IsValidSlot(stealthSlotIndex) ? weaponSlots[stealthSlotIndex].gameObject.activeSelf : (bool?)null)}");

    }

    private Shooter CreateShooter(GunData data)
    {
        var parent = (GameManager.Instance && GameManager.Instance.Player) ? GameManager.Instance.Player : transform;
        var go = new GameObject($"Shooter_{data.name}");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        var shooter = go.AddComponent<Shooter>();
        shooter.Initialize(data);
        shooter.shooterLocked = _globalShooterLocked; // 전역 잠금 상태 상속
        go.SetActive(false);
        return shooter;
    }

    // === 로드아웃 → 슬롯 매핑 ===
    public void ApplyLoadoutFromProfile()
    {
        stealthSlotIndex = -1;
        combatSlotIndex = -1;

        var lp = LoadoutProfile.Instance;
        var inv = WeaponInventory.Instance;

        // 1) id → GunData
        GunData stealth = (lp && inv) ? inv.FindById(lp.GetStealthId()) : null;
        GunData combat = (lp && inv) ? inv.FindById(lp.GetCombatId()) : null;

        // 2) 슬롯 인덱스 찾기
        int FindIndex(GunData target)
        {
            if (!target) return -1;
            for (int i = 0; i < weaponSlots.Count; i++)
                if (weaponSlots[i] && weaponSlots[i].gunData == target) return i;
            return -1;
        }

        stealthSlotIndex = FindIndex(stealth);
        combatSlotIndex = FindIndex(combat);

        // 3) 자동 대체 규칙(타깃 없을 때)
        if (stealthSlotIndex < 0) stealthSlotIndex = PickByPhaseTag(GunData.PhaseTag.Stealth, preferAny: true);
        if (combatSlotIndex < 0) combatSlotIndex = PickByPhaseTag(GunData.PhaseTag.Combat, preferAny: true, avoid: stealthSlotIndex);
        if (stealthSlotIndex < 0) stealthSlotIndex = FirstValidSlot();
        if (combatSlotIndex < 0) combatSlotIndex = FirstValidSlot(avoid: stealthSlotIndex);
    }

    private int PickByPhaseTag(GunData.PhaseTag tag, bool preferAny, int avoid = -1)
    {
        int AnyMatch(Func<GunData, bool> pred)
        {
            for (int i = 0; i < weaponSlots.Count; i++)
            {
                var s = weaponSlots[i]; if (!s || i == avoid) continue;
                var d = s.gunData; if (d && pred(d)) return i;
            }
            return -1;
        }
        int idx = AnyMatch(d => d.phaseTag == tag);
        if (idx >= 0) return idx;
        if (preferAny) idx = AnyMatch(d => d.phaseTag == GunData.PhaseTag.Any);
        return idx;
    }

    // === 페이즈/장착 ===
    public void ApplyPhaseWeapon(GamePhase phase)
    {
        if (weaponSlots.Count == 0) return;

        int target = (phase == GamePhase.Combat) ? combatSlotIndex : stealthSlotIndex;
        if (!IsValidSlot(target)) return;

        // 인덱스가 같아도 오브젝트가 꺼져 있으면 장착(활성) 강제
        if (currentIndex != target || !weaponSlots[target].gameObject.activeSelf)
            EquipByIndex(target);
    }


    public bool EquipByIndex(int slot)
    {
        if (!IsValidSlot(slot)) return false;

        if (IsValidSlot(currentIndex))
            weaponSlots[currentIndex].gameObject.SetActive(false);

        currentIndex = slot;
        weaponSlots[currentIndex].gameObject.SetActive(true);

        OnWeaponChanged.Invoke(CurrentWeapon);
        OnAmmoChanged.Invoke(); // HUD 즉시 갱신
        return true;
    }

    public bool EquipByData(GunData data)
    {
        if (!data) return false;
        for (int i = 0; i < weaponSlots.Count; i++)
            if (weaponSlots[i] && weaponSlots[i].gunData == data)
                return EquipByIndex(i);
        return false;
    }

    // === 상점 구매 이벤트: 풀 전체 리빌드 없이 "Shooter 1개만" 증설 ===
    private void OnItemBought(GunData data)
    {
        if (!data) return;

        var shooter = CreateShooter(data);
        int newIdx = weaponSlots.Count;
        weaponSlots.Add(shooter);

        // 빈 슬롯 자동 채움
        bool stealthOK = data.phaseTag == GunData.PhaseTag.Stealth || data.phaseTag == GunData.PhaseTag.Any;
        bool combatOK = data.phaseTag == GunData.PhaseTag.Combat || data.phaseTag == GunData.PhaseTag.Any;
        if (stealthSlotIndex < 0 && stealthOK) stealthSlotIndex = newIdx;
        if (combatSlotIndex < 0 && combatOK) combatSlotIndex = newIdx;
    }

    // === 전역 발사 잠금 ===
    public void SetShooterLocked(bool locked)
    {
        _globalShooterLocked = locked;
        foreach (var s in weaponSlots) if (s) s.shooterLocked = locked;
    }

    // === 탄약/리로드 헬퍼 ===
    public int GetMagazine() => CurrentWeapon ? CurrentWeapon.CurrentMagazine : 0;
    public int GetReserve() => CurrentWeapon ? CurrentWeapon.CurrentAmmo : 0;
    public bool RequestReload() => CurrentWeapon && CurrentWeapon.Reload();

    public int AddAmmoToCurrentPhase(int amount)
    {
        var gm = GameManager.Instance;
        var phase = gm ? gm.CurrentPhase : GamePhase.Stealth;
        return AddAmmoToPhase(phase, amount);
    }
    public int AddAmmoToPhase(GamePhase phase, int amount)
    {
        var s = (phase == GamePhase.Combat) ? GetCombatShooter() : GetStealthShooter();
        if (!s) return 0;
        int added = s.AddAmmo(amount); // Shooter.cs의 API
        OnAmmoChanged.Invoke();
        return added;
    }

    public Shooter GetStealthShooter() => IsValidSlot(stealthSlotIndex) ? weaponSlots[stealthSlotIndex] : null;
    public Shooter GetCombatShooter() => IsValidSlot(combatSlotIndex) ? weaponSlots[combatSlotIndex] : null;

    // === Utils ===
    private bool IsValidSlot(int idx) => idx >= 0 && idx < weaponSlots.Count && weaponSlots[idx];
    private int FirstValidSlot(int avoid = -1)
    {
        for (int i = 0; i < weaponSlots.Count; i++)
            if (weaponSlots[i] && i != avoid) return i;
        return -1;
    }
}
