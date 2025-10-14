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
    [SerializeField] private int stealthSlotIndex = -1; // 무조건 권총류
    [SerializeField] private int combatSlotIndex = -1;  // 라이플/샷건/SMG/MG/스나 중 택1

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
            if (value == currentIndex) return; // 동일 인덱스면 아무 것도 하지 않음 (루프 방지)

            // 인덱스 변경 (탄약은 각 무기가 독립적으로 보유)
            currentIndex = Mathf.Clamp(value, 0, weaponSlots.Length - 1);

            // 이벤트 알림
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

        if (phase == Constants.GamePhase.Combat)
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
        // 씬 전환 시 무기 초기화
        InitializeWeapon();
    }

    public void InitializeWeapon()
    {
        if (weaponSlots != null)
        {
            foreach (var weapon in weaponSlots)
            {
                if (weapon != null)
                    Destroy(weapon.gameObject);
            }
        }

        weaponSlots = new Shooter[intializeDatas.Length];

        for (int i = 0; i < intializeDatas.Length; i++)
        {
            var data = intializeDatas[i];
            var go = new GameObject(data.displayName);
            go.transform.SetParent(GameManager.Instance.Player);
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            var shooter = go.AddComponent<Shooter>();
            shooter.Initialize(data); // Shooter가 GunData의 탄약으로 초기화
            weaponSlots[i] = shooter;
        }

        // 기본 로드아웃 자동 선택
        PickDefaultLoadoutIndices();

        CurrentWeaponIndex = initializeIndex;
        OnWeaponChanged.Invoke(CurrentWeapon);
        OnAmmoChanged.Invoke();

        // 현재 페이즈 무기 적용
        ApplyPhaseWeapon(GameManager.Instance.CurrentPhase);
    }

    private void PickDefaultLoadoutIndices()
    {
        // 1) 스텔스(권총) — 첫 Pistol
        stealthSlotIndex = -1;
        for (int i = 0; i < intializeDatas.Length; i++)
        {
            if (intializeDatas[i] != null && intializeDatas[i].weaponClass == GunData.WeaponClass.Pistol)
            {
                stealthSlotIndex = i;
                break;
            }
        }

        // 2) 난전(비권총) — Rifle > SMG > Shotgun > MG > Sniper 우선
        combatSlotIndex = -1;
        int PickFirst(GunData.WeaponClass c)
        {
            for (int i = 0; i < intializeDatas.Length; i++)
            {
                if (intializeDatas[i] != null && intializeDatas[i].weaponClass == c) return i;
            }
            return -1;
        }

        int idx = PickFirst(GunData.WeaponClass.Rifle);
        if (idx < 0) idx = PickFirst(GunData.WeaponClass.SMG);
        if (idx < 0) idx = PickFirst(GunData.WeaponClass.Shotgun);
        if (idx < 0) idx = PickFirst(GunData.WeaponClass.MG);
        if (idx < 0) idx = PickFirst(GunData.WeaponClass.Sniper);
        combatSlotIndex = idx;

        // initializeIndex가 유효하지 않다면 스텔스 우선
        if (!IsValidSlot(initializeIndex))
            initializeIndex = (IsValidSlot(stealthSlotIndex) ? stealthSlotIndex : 0);
    }

    private bool IsValidSlot(int i) => (i >= 0 && weaponSlots != null && i < weaponSlots.Length);

    // ---- Loadout Selection APIs (무기방 UI에서 호출) ----
    /// <summary>권총류만 허용. 성공 시 true</summary>
    public bool SetStealthSlot(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return false;
        var data = intializeDatas[slotIndex];
        if (data.weaponClass != GunData.WeaponClass.Pistol) return false;
        stealthSlotIndex = slotIndex;
        if (GameManager.Instance.CurrentPhase == GamePhase.Stealth)
            CurrentWeaponIndex = stealthSlotIndex;
        return true;
    }

    /// <summary>비권총(라이플/샷건/SMG/MG/스나)만 허용. 성공 시 true</summary>
    public bool SetCombatSlot(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return false;
        var cls = intializeDatas[slotIndex].weaponClass;
        if (cls == GunData.WeaponClass.Pistol) return false;
        combatSlotIndex = slotIndex;
        if (GameManager.Instance.CurrentPhase == GamePhase.Combat)
            CurrentWeaponIndex = combatSlotIndex;
        return true;
    }

    public void SetShooterLocked(bool locked)
    {
        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot != null)
                weaponSlot.shooterLocked = locked;
        }
    }

    public int GetMagazine() => CurrentWeapon ? CurrentWeapon.CurrentMagazine : 0;
    public int GetReserve() => CurrentWeapon ? CurrentWeapon.CurrentAmmo : 0;
    public bool RequestReload() => CurrentWeapon && CurrentWeapon.Reload();

    // ---- Ammo API (픽업/상점은 여기만 호출) ----

    public Shooter GetShooterAt(int slotIndex)
        => IsValidSlot(slotIndex) ? weaponSlots[slotIndex] : null;

    public Shooter GetStealthShooter() => GetShooterAt(stealthSlotIndex);
    public Shooter GetCombatShooter() => GetShooterAt(combatSlotIndex);

    /// <summary>현재 페이즈(잠입/난전)용 무기에 예비탄 추가. Knife 들고 있어도 해당 페이즈 무기에 들어감.</summary>
    public int AddAmmoToCurrentPhase(int amount)
    {
        var phase = GameManager.Instance.CurrentPhase;
        return AddAmmoToPhase(phase, amount);
    }

    /// <summary>지정 페이즈용 무기에 예비탄 추가.</summary>
    public int AddAmmoToPhase(GamePhase phase, int amount)
    {
        Shooter s = (phase == GamePhase.Combat) ? GetCombatShooter() : GetStealthShooter();
        if (s == null) return 0;
        int gained = s.AddAmmo(amount);
        return gained;
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
}
