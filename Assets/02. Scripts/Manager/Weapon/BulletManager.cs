using System;
using UnityEngine;
using UnityEngine.Events;
using Constants;

public class BulletManager : Singleton<BulletManager>
{
    [Serializable]
    public struct WeaponSlot
    {
        public WeaponType type;
        public int magazineCapacity;
        public int currentMagazine;
        public int reserve;
    }

    [Header("ź�� ����")]
    [SerializeField]
    private WeaponSlot pistol = new()
    {
        type = WeaponType.Pistol,
        magazineCapacity = 12,
        currentMagazine = 12,
        reserve = 36
    };
    [SerializeField]
    private WeaponSlot rifle = new()
    {
        type = WeaponType.Rifle,
        magazineCapacity = 30,
        currentMagazine = 30,
        reserve = 90
    };

    public UnityEvent OnAmmoChanged;   // HUD�� ����
    public UnityEvent OnReloaded;

    // ---- �ܺ� API ----
    public bool TryConsumeOneBullet()
    {
        var type = GetCurrentType();
        ref var slot = ref GetSlot(type);
        if (slot.currentMagazine <= 0) return false;

        slot.currentMagazine--;
        OnAmmoChanged?.Invoke();
        return true;
    }

    public void RequestReload()
    {
        var type = GetCurrentType();
        ref var slot = ref GetSlot(type);

        if (slot.currentMagazine >= slot.magazineCapacity) return;
        if (slot.reserve <= 0) return;

        int need = slot.magazineCapacity - slot.currentMagazine;
        int take = Mathf.Min(need, slot.reserve);

        slot.reserve -= take;
        slot.currentMagazine += take;

        OnAmmoChanged?.Invoke();
        OnReloaded?.Invoke();
    }

    // ��ȸ��
    public int GetMagazine(WeaponType type) => GetSlot(type).currentMagazine;
    public int GetReserve(WeaponType type) => GetSlot(type).reserve;

    //public int GetMagazineCurrent() => GetMagazine(GetCurrentType());
    //public int GetReserveCurrent() => GetReserve(GetCurrentType());

    // ---- ���� ----
    private WeaponType GetCurrentType()
    {
        return WeaponManager.Instance ? WeaponManager.Instance.CurrentWeapon : WeaponType.Pistol;
    }

    private ref WeaponSlot GetSlot(WeaponType type)
    {
        if (type == WeaponType.Rifle) return ref rifle;
        return ref pistol;
    }
}
