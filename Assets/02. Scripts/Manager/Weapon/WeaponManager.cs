using System;
using UnityEngine;
using UnityEngine.Events;
using Constants;

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField] private WeaponType currentWeapon = WeaponType.Pistol;
    public WeaponType CurrentWeapon => currentWeapon;

    [Serializable] public class WeaponChangedEvent : UnityEvent<WeaponType> { }
    public WeaponChangedEvent OnWeaponChanged = new();         // HUD/애니/사운드가 구독

    private void Start()
    {
        OnWeaponChanged.Invoke(currentWeapon);
    }


    // 명시 전환
    public void SetWeapon(WeaponType type)
    {
        if (currentWeapon == type) return;
        currentWeapon = type;
        OnWeaponChanged.Invoke(currentWeapon);
    }

    public void Toggle()
    {
        var next = (currentWeapon == WeaponType.Pistol) ? WeaponType.Rifle : WeaponType.Pistol;
        SetWeapon(next);
    }
}
