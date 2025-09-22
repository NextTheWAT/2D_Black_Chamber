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
    public WeaponChangedEvent OnWeaponChanged = new();         // HUD/�ִ�/���尡 ����

    public UnityEvent OnAmmoChanged;   // HUD�� ����
    public UnityEvent OnReloaded;

    private void Start()
    {
        SetWeapon(CurrentWeapon);
    }

    public void ConnectPlayerShooter(Shooter shooter)
        => playerShooter = shooter;

    // ��� ��ȯ
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
