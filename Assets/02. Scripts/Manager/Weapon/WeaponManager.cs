using Constants;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WeaponManager : Singleton<WeaponManager>
{
    [Serializable] public class WeaponChangedEvent : UnityEvent<Shooter> { }
    public WeaponChangedEvent OnWeaponChanged = new();         // HUD/애니/사운드가 구독

    public UnityEvent OnAmmoChanged { get; private set; } = new UnityEvent();
    public UnityEvent OnReloaded { get; private set; } = new UnityEvent();

    [Header("Initialize")]
    public GunData[] intializeDatas;
    [HideInInspector] public Shooter[] weaponSlots;

    [SerializeField] private int initializeIndex = 0; // 초기 무기 인덱스
    private int currentIndex = 0;

    // ---- Phase Loadouts ----
    [Header("Phase Loadout (Stealth/Combat)")]
    [SerializeField] private int stealthSlotIndex = -1; // PhaseTag.Stealth(또는 Any) 우선
    [SerializeField] private int combatSlotIndex = -1;  // PhaseTag.Combat(또는 Any) 우선

    public int StealthSlotIndex => stealthSlotIndex;
    public int CombatSlotIndex => combatSlotIndex;

    public Shooter CurrentWeapon
        => (weaponSlots != null && weaponSlots.Length > 0) ? weaponSlots[currentIndex] : null;

    public int CurrentWeaponIndex
    {
        get => currentIndex;
        set
        {
            if (weaponSlots == null || weaponSlots.Length == 0) return;
            if (value == currentIndex) return;

            // 이전 무기 끄기
            if (IsValidSlot(currentIndex))
                weaponSlots[currentIndex].gameObject.SetActive(false);

            currentIndex = Mathf.Clamp(value, 0, weaponSlots.Length - 1);

            // 새 무기 켜기
            if (IsValidSlot(currentIndex))
                weaponSlots[currentIndex].gameObject.SetActive(true);

            OnWeaponChanged.Invoke(CurrentWeapon);
            OnAmmoChanged.Invoke();
        }
    }


    private void OnEnable()
    {
        if (AppIsQuitting) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDisable()
    {
        if (AppIsQuitting) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(GamePhase phase)
    {
        ApplyPhaseWeapon(phase);
    }

    public void ApplyPhaseWeapon(GamePhase phase)
    {
        if (weaponSlots == null || weaponSlots.Length == 0) return;

        if (phase == GamePhase.Combat)
        {
            if (IsValidSlot(combatSlotIndex) && CurrentWeaponIndex != combatSlotIndex)
                CurrentWeaponIndex = combatSlotIndex;
        }
        else
        {
            if (IsValidSlot(stealthSlotIndex) && CurrentWeaponIndex != stealthSlotIndex)
                CurrentWeaponIndex = stealthSlotIndex;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeWeapon();
    }

    public void InitializeWeapon()
    {
        // 기존 무기 정리
        if (weaponSlots != null)
        {
            foreach (var weapon in weaponSlots)
                if (weapon != null) Destroy(weapon.gameObject);
        }

        // 방어
        int len = (intializeDatas != null) ? intializeDatas.Length : 0;
        if (len <= 0)
        {
            weaponSlots = Array.Empty<Shooter>();
            return;
        }

        // 슬롯 생성
        weaponSlots = new Shooter[len];

        for (int i = 0; i < len; i++)
        {
            var data = intializeDatas[i];
            if (data == null) continue;

            var go = new GameObject(data.displayName);
            go.transform.SetParent(GameManager.Instance.Player);
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            var shooter = go.AddComponent<Shooter>();
            shooter.Initialize(data); // Shooter가 GunData의 탄약/파라미터로 초기화
            weaponSlots[i] = shooter;
        }

        // 기본 로드아웃 자동 선택(PhaseTag 기반)
        PickDefaultLoadoutIndices();

        // 초깃값 보정
        if (!IsValidSlot(initializeIndex))
            initializeIndex = IsValidSlot(stealthSlotIndex) ? stealthSlotIndex : 0;

        CurrentWeaponIndex = initializeIndex;
        OnWeaponChanged.Invoke(CurrentWeapon);
        OnAmmoChanged.Invoke();

        ApplyPhaseWeapon(GameManager.Instance.CurrentPhase);
    }

    // ---- Default pick by PhaseTag ----
    private void PickDefaultLoadoutIndices()
    {
        stealthSlotIndex = -1;
        combatSlotIndex = -1;

        // 태그 우선 선택 헬퍼
        int TryPickByTag(GunData.PhaseTag tag)
        {
            if (intializeDatas == null) return -1;
            for (int i = 0; i < intializeDatas.Length; i++)
            {
                var d = intializeDatas[i];
                if (d == null) continue;
                if (d.phaseTag == tag) return i;
            }
            return -1;
        }

        // 1) 스텔스: Stealth 태그 우선 → 없으면 Any → 없으면 첫 유효 무기
        stealthSlotIndex = TryPickByTag(GunData.PhaseTag.Stealth);
        if (stealthSlotIndex < 0)
        {
            // Any
            for (int i = 0; i < intializeDatas.Length; i++)
            {
                var d = intializeDatas[i];
                if (d != null && d.phaseTag == GunData.PhaseTag.Any)
                {
                    stealthSlotIndex = i;
                    break;
                }
            }
        }
        if (stealthSlotIndex < 0)
        {
            // 첫 유효 무기
            for (int i = 0; i < intializeDatas.Length; i++)
                if (intializeDatas[i] != null) { stealthSlotIndex = i; break; }
        }

        // 2) 난전: Combat 태그 우선 → 없으면 Any(스텔스와 다른 Any 시도) → 없으면 첫 유효 무기
        combatSlotIndex = TryPickByTag(GunData.PhaseTag.Combat);
        if (combatSlotIndex < 0)
        {
            for (int i = 0; i < intializeDatas.Length; i++)
            {
                var d = intializeDatas[i];
                if (d != null && d.phaseTag == GunData.PhaseTag.Any)
                {
                    // 스텔스와 같은 인덱스를 피하려 시도(가능하면)
                    if (i != stealthSlotIndex) { combatSlotIndex = i; break; }
                }
            }
        }
        if (combatSlotIndex < 0)
        {
            for (int i = 0; i < intializeDatas.Length; i++)
                if (intializeDatas[i] != null && i != stealthSlotIndex) { combatSlotIndex = i; break; }
        }
    }

    private bool IsValidSlot(int i)
        => (i >= 0 && weaponSlots != null && i < weaponSlots.Length && weaponSlots[i] != null);

    // ---- Loadout Selection APIs (무기방 UI에서 호출) ----
    // PhaseTag 기반 검증: Stealth/Combat 혹은 Any 허용
    private static bool IsStealthCompatible(GunData d)
        => d != null && (d.phaseTag == GunData.PhaseTag.Stealth || d.phaseTag == GunData.PhaseTag.Any);

    private static bool IsCombatCompatible(GunData d)
        => d != null && (d.phaseTag == GunData.PhaseTag.Combat || d.phaseTag == GunData.PhaseTag.Any);

    /// <summary>Stealth 슬롯 교체 (PhaseTag.Stealth/Any만 허용). 성공 시 true</summary>
    public bool SetStealthSlot(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return false;
        var data = intializeDatas[slotIndex];
        if (!IsStealthCompatible(data)) return false;

        stealthSlotIndex = slotIndex;
        if (GameManager.Instance.CurrentPhase == GamePhase.Stealth)
            CurrentWeaponIndex = stealthSlotIndex;
        return true;
    }

    /// <summary>Combat 슬롯 교체 (PhaseTag.Combat/Any만 허용). 성공 시 true</summary>
    public bool SetCombatSlot(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return false;
        var data = intializeDatas[slotIndex];
        if (!IsCombatCompatible(data)) return false;

        combatSlotIndex = slotIndex;
        if (GameManager.Instance.CurrentPhase == GamePhase.Combat)
            CurrentWeaponIndex = combatSlotIndex;
        return true;
    }

    public void SetShooterLocked(bool locked)
    {
        if (weaponSlots == null) return;
        foreach (var weaponSlot in weaponSlots)
            if (weaponSlot != null) weaponSlot.shooterLocked = locked;
    }

    public int GetMagazine() => CurrentWeapon ? CurrentWeapon.CurrentMagazine : 0;
    public int GetReserve() => CurrentWeapon ? CurrentWeapon.CurrentAmmo : 0;
    public bool RequestReload() => CurrentWeapon && CurrentWeapon.Reload();

    // ---- Ammo API (픽업/상점은 여기만 호출) ----
    public Shooter GetShooterAt(int slotIndex)
        => IsValidSlot(slotIndex) ? weaponSlots[slotIndex] : null;

    public Shooter GetStealthShooter() => GetShooterAt(stealthSlotIndex);
    public Shooter GetCombatShooter() => GetShooterAt(combatSlotIndex);

    /// <summary>현재 페이즈(잠입/난전)용 무기에 예비탄 추가.</summary>
    public int AddAmmoToCurrentPhase(int amount)
        => AddAmmoToPhase(GameManager.Instance.CurrentPhase, amount);

    /// <summary>지정 페이즈용 무기에 예비탄 추가.</summary>
    public int AddAmmoToPhase(GamePhase phase, int amount)
    {
        Shooter s = (phase == GamePhase.Combat) ? GetCombatShooter() : GetStealthShooter();
        if (s == null) return 0;
        return s.AddAmmo(amount);
    }

    /// <summary>슬롯 인덱스 기준으로 예비탄 추가.</summary>
    public int AddAmmoToSlot(int slotIndex, int amount)
    {
        var s = GetShooterAt(slotIndex);
        if (s == null) return 0;
        return s.AddAmmo(amount);
    }

    /// <summary>지정 페이즈용 무기에 탄이 있는지 확인(탄창+예비 합)</summary>
    public bool HasAmmoForPhase(GamePhase phase)
    {
        Shooter s = (phase == GamePhase.Combat) ? GetCombatShooter() : GetStealthShooter();
        if (s == null) return false;
        return (s.CurrentMagazine + s.CurrentAmmo) > 0;
    }


    private Shooter CreateShooter(GunData data)
    {
        var go = new GameObject(data.displayName);
        go.transform.SetParent(GameManager.Instance.Player);
        go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        var shooter = go.AddComponent<Shooter>();
        shooter.Initialize(data);
        return shooter;
    }

    /// <summary>
    /// 인게임 도중 새 무기를 추가한다(상점 구매 등).
    /// - intializeDatas / weaponSlots 둘 다 확장
    /// - 현재 페이즈와 태그가 맞으면 자동 장착(옵션)
    /// </summary>
    public Shooter AddWeapon(GunData data, bool autoEquip = true)
    {
        if (data == null) return null;

        // 1) weaponSlots 확장 + Shooter 생성
        int oldLen = (weaponSlots != null) ? weaponSlots.Length : 0;
        var newSlots = new Shooter[oldLen + 1];
        if (oldLen > 0) weaponSlots.CopyTo(newSlots, 0);
        var shooter = CreateShooter(data);
        newSlots[oldLen] = shooter;
        weaponSlots = newSlots;

        // 2) intializeDatas 확장 (씬 로드시 재생성용)
        int oldDataLen = (intializeDatas != null) ? intializeDatas.Length : 0;
        var newDatas = new GunData[oldDataLen + 1];
        if (oldDataLen > 0) intializeDatas.CopyTo(newDatas, 0);
        newDatas[oldDataLen] = data;
        intializeDatas = newDatas;

        int newIndex = oldLen;

        // 3) 슬롯 힌트(처음 추가될 경우만 기본 슬롯 세팅)
        if (data.phaseTag == GunData.PhaseTag.Stealth && stealthSlotIndex < 0) stealthSlotIndex = newIndex;
        if (data.phaseTag == GunData.PhaseTag.Combat && combatSlotIndex < 0) combatSlotIndex = newIndex;

        // 4) 자동 장착 (현재 페이즈와 태그가 맞으면)
        if (autoEquip)
        {
            var phase = GameManager.Instance.CurrentPhase;
            bool stealthOK = data.phaseTag == GunData.PhaseTag.Stealth || data.phaseTag == GunData.PhaseTag.Any;
            bool combatOK = data.phaseTag == GunData.PhaseTag.Combat || data.phaseTag == GunData.PhaseTag.Any;

            if (phase == GamePhase.Combat && combatOK)
                CurrentWeaponIndex = newIndex;
            else if (phase != GamePhase.Combat && stealthOK)
                CurrentWeaponIndex = newIndex;
        }

        OnWeaponChanged.Invoke(CurrentWeapon);
        OnAmmoChanged.Invoke();
        return shooter;
    }
}
