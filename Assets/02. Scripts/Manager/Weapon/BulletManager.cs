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
        public int magazineCapacity;     // 탄창 용량
        public int currentMagazine;      // 현재 탄창(쏠 수 있는 총알)
        public int reserve;              // 남아있는 예비탄
    }

    [Header("탄약 설정 (원하면 인스펙터에서 수정)")]
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

    [Header("현재 상태")]
    [SerializeField] private WeaponType currentWeapon = WeaponType.Pistol;

    //[Header("선택: 리로드 애니메이션 재생 연결")]
    //[SerializeField] private CharacterAnimationController animationController;

    // UI는 여기만 바라보면 됨
    public UnityEvent OnAmmoChanged;   // 탄 수 바뀔 때 호출
    public UnityEvent OnReloaded;      // 리로드 완료 시 호출

    // ========== 외부 API ==========
    public void SetCurrentWeapon(WeaponType type)
    {
        currentWeapon = type;
        OnAmmoChanged?.Invoke();
    }

    /// <summary>발사 시 탄 1발 소비. 성공 true / 실패(빈 탄창) false</summary>
    public bool TryConsumeOneBullet()
    {
        ref var slot = ref GetSlot(currentWeapon);
        if (slot.currentMagazine <= 0) return false;

        slot.currentMagazine--;
        OnAmmoChanged?.Invoke();
        return true;
    }

    /// <summary>R 입력 훅: 즉시 리로드. 예비탄이 허용하는 한에서만 채움</summary>
    public void RequestReload()
    {
        ref var slot = ref GetSlot(currentWeapon);

        if (slot.currentMagazine >= slot.magazineCapacity) return; // 이미 가득
        if (slot.reserve <= 0) return;                             // 예비탄 없음

        int need = slot.magazineCapacity - slot.currentMagazine;
        int take = Mathf.Min(need, slot.reserve);

        slot.reserve -= take;
        slot.currentMagazine += take;

        // 선택: 애니메이션/사운드 재생 (있으면 연결, 아니면 null이므로 무시됨)
        //animationController?.PlayReload();

        OnAmmoChanged?.Invoke();
        OnReloaded?.Invoke();
    }

    // UI에서 바로 쓰기 편하도록 getter 제공
    //public int GetMagazine() => GetSlot(currentWeapon).currentMagazine;
    //public int GetReserve() => GetSlot(currentWeapon).reserve;
    public int GetMagazine(WeaponType type) => GetSlot(type).currentMagazine;
    public int GetReserve(WeaponType type) => GetSlot(type).reserve;

    // ========== 내부 ==========
    private ref WeaponSlot GetSlot(WeaponType type)
    {
        if (type == WeaponType.Rifle) return ref rifle;
        return ref pistol;
    }

}
