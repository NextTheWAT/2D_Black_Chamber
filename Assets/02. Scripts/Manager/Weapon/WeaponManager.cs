using System;
using UnityEngine;
using UnityEngine.Events;
using Constants;

public class WeaponManager : Singleton<WeaponManager>
{
    public Shooter CurrentWeapon => weaponSlots[currentIndex];

    public GunData[] intializeDatas;
    [HideInInspector] public Shooter[] weaponSlots;
    public int currentIndex = 0;

    [Serializable] public class WeaponChangedEvent : UnityEvent<Shooter> { }
    public WeaponChangedEvent OnWeaponChanged = new();         // HUD/�ִ�/���尡 ����

    public UnityEvent OnAmmoChanged;   // HUD�� ����
    public UnityEvent OnReloaded;

    public int CurrentWeaponIndex
    {
        get => currentIndex;
        set
        {
            if(currentIndex == value) return; // ���� ���� ���� ����
            if (value < 0 || value >= weaponSlots.Length) return; // ���� �˻�

            currentIndex = value;

            OnWeaponChanged.Invoke(CurrentWeapon);
        }
    }

    private void Start()
    {
        CurrentWeaponIndex = 0;

        // ���� �ʱ�ȭ
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
    }

    public int GetMagazine() => CurrentWeapon.CurrentMagazine;
    public int GetReserve() => CurrentWeapon.CurrentAmmo;
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
