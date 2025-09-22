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

    

    // ---- �ܺ� API ----

    /*
    public void RequestReload()
    {
        playerShooter.Reload();
    }

    // ��ȸ��
    public int GetMagazine() => playerShooter.CurrentMagazine;
    public int GetReserve() => playerShooter.CurrentAmmo;
    */

    //public int GetMagazineCurrent() => GetMagazine(GetCurrentType());
    //public int GetReserveCurrent() => GetReserve(GetCurrentType());
}
