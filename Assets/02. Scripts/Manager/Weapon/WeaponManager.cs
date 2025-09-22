using System;
using UnityEngine;
using UnityEngine.Events;
using Constants;

public class WeaponManager : Singleton<WeaponManager>
{
    public GunData CurrentWeapon => weaponSlots[currentIndex];

    public GunData[] weaponSlots;
    public int currentIndex = 0;
    private Shooter playerShooter;

    [Serializable] public class WeaponChangedEvent : UnityEvent<GunData> { }
    public WeaponChangedEvent OnWeaponChanged = new();         // HUD/애니/사운드가 구독

    public UnityEvent OnAmmoChanged;   // HUD가 구독
    public UnityEvent OnReloaded;

    private void Start()
    {
        SetWeapon(CurrentWeapon);
    }

    public void ConnectPlayerShooter(Shooter shooter)
        => playerShooter = shooter;

    // 명시 전환
    public void SetWeapon(GunData gunData)
    {
        playerShooter.Initialize(gunData);
        OnWeaponChanged.Invoke(gunData);
    }
    public int GetMagazine() => playerShooter.CurrentMagazine;
    public int GetReserve() => playerShooter.CurrentAmmo;
    public void RequestReload() => playerShooter.Reload();

    public void Toggle()
    {
        currentIndex = (currentIndex + 1) % weaponSlots.Length;
        SetWeapon(weaponSlots[currentIndex]);
        /*
        var next = (currentWeapon == WeaponType.Pistol) ? WeaponType.Rifle : WeaponType.Pistol;
        SetWeapon(next);
        */
    }
}
