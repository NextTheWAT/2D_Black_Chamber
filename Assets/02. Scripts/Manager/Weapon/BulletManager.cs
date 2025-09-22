using System;
using UnityEngine;
using UnityEngine.Events;
using Constants;

public class BulletManager : Singleton<BulletManager>
{
    [Serializable]
    public struct WeaponSlot
    {
        public WeaponType type;          // Pistol / Rifle
        public int magazineCapacity;     // źâ �뷮
        public int currentMagazine;      // ���� źâ(�� �� �ִ� �Ѿ�)
        public int reserve;              // �����ִ� ����ź
    }

    [Header("ź�� ���� (���ϸ� �ν����Ϳ��� ����)")]
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

    [Header("���� ����")]
    [SerializeField] private WeaponType currentWeapon = WeaponType.Pistol;

    //[Header("����: ���ε� �ִϸ��̼� ��� ����")]
    //[SerializeField] private CharacterAnimationController animationController;

    // UI�� ���⸸ �ٶ󺸸� ��
    public UnityEvent OnAmmoChanged;   // ź �� �ٲ� �� ȣ��
    public UnityEvent OnReloaded;      // ���ε� �Ϸ� �� ȣ��

    // ========== �ܺ� API ==========
    public void SetCurrentWeapon(WeaponType type)
    {
        currentWeapon = type;
        OnAmmoChanged?.Invoke();
    }

    /// <summary>�߻� �� ź 1�� �Һ�. ���� true / ����(�� źâ) false</summary>
    public bool TryConsumeOneBullet()
    {
        ref var slot = ref GetSlot(currentWeapon);
        if (slot.currentMagazine <= 0) return false;

        slot.currentMagazine--;
        OnAmmoChanged?.Invoke();
        return true;
    }

    /// <summary>R �Է� ��: ��� ���ε�. ����ź�� ����ϴ� �ѿ����� ä��</summary>
    public void RequestReload()
    {
        ref var slot = ref GetSlot(currentWeapon);

        if (slot.currentMagazine >= slot.magazineCapacity) return; // �̹� ����
        if (slot.reserve <= 0) return;                             // ����ź ����

        int need = slot.magazineCapacity - slot.currentMagazine;
        int take = Mathf.Min(need, slot.reserve);

        slot.reserve -= take;
        slot.currentMagazine += take;

        // ����: �ִϸ��̼�/���� ��� (������ ����, �ƴϸ� null�̹Ƿ� ���õ�)
        //animationController?.PlayReload();

        OnAmmoChanged?.Invoke();
        OnReloaded?.Invoke();
    }

    // UI���� �ٷ� ���� ���ϵ��� getter ����
    //public int GetMagazine() => GetSlot(currentWeapon).currentMagazine;
    //public int GetReserve() => GetSlot(currentWeapon).reserve;
    public int GetMagazine(WeaponType type) => GetSlot(type).currentMagazine;
    public int GetReserve(WeaponType type) => GetSlot(type).reserve;

    // ========== ���� ==========
    private ref WeaponSlot GetSlot(WeaponType type)
    {
        if (type == WeaponType.Rifle) return ref rifle;
        return ref pistol;
    }

}
