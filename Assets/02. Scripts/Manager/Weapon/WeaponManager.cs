using System;
using UnityEngine;
using UnityEngine.Events;
using Constants;

public class WeaponManager : Singleton<WeaponManager>
{
    public Shooter CurrentWeapon => 
        (weaponSlots != null && weaponSlots.Length > 0) ? weaponSlots[currentIndex] : null;

    public GunData[] intializeDatas;
    [HideInInspector] public Shooter[] weaponSlots;
    public int currentIndex = 0;

    [Serializable] public class WeaponChangedEvent : UnityEvent<Shooter> { }
    public WeaponChangedEvent OnWeaponChanged = new();         // HUD/애니/사운드가 구독

    public UnityEvent OnAmmoChanged { get; private set; } = new UnityEvent();
    public UnityEvent OnReloaded { get; private set; } = new UnityEvent();


    public int CurrentWeaponIndex
    {
        get => currentIndex;
        set
        {
            if(currentIndex == value) return; // 같은 무기 선택 방지
            if (value < 0 || value >= weaponSlots.Length) return; // 범위 검사

            currentIndex = value;

            OnWeaponChanged.Invoke(CurrentWeapon);
        }
    }

    private void Start()
    {
        CurrentWeaponIndex = 0;

        weaponSlots = new Shooter[intializeDatas.Length];

        for (int i = 0; i < intializeDatas.Length; i++)
        {
            var go = new GameObject(intializeDatas[i].displayName);
            go.transform.SetParent(GameManager.Instance.player);
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            var shooter = go.AddComponent<Shooter>();
            shooter.Initialize(intializeDatas[i]);
            weaponSlots[i] = shooter;
        }

        currentIndex = 0;

        OnWeaponChanged?.Invoke(CurrentWeapon);
        OnAmmoChanged?.Invoke();
    }

    public int GetMagazine() => CurrentWeapon ? CurrentWeapon.CurrentMagazine : 0;
    public int GetReserve() => CurrentWeapon ? CurrentWeapon.CurrentAmmo : 0;
    public void RequestReload() => CurrentWeapon.Reload();

    public void Toggle()
    {
        CurrentWeaponIndex = (currentIndex + 1) % weaponSlots.Length;
        /*
        var next = (currentWeapon == WeaponType.Pistol) ? WeaponType.Rifle : WeaponType.Pistol;
        SetWeapon(next);
        */
    }
}
