using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Gun/Gun Data (Extended)")]
public class GunData : ScriptableObject
{
    // ===== Classification =====
    public enum WeaponClass { Pistol, Shotgun, SMG, Rifle, MG, Sniper, Knife }
    public enum NoiseLevel { Silent, Suppressed, Normal, Loud }

    [Header("Classification")]
    public string displayName = "Pistol";
    public WeaponClass weaponClass = WeaponClass.Pistol;
    [Tooltip("세부 분류 (예: AR, DMR, Carbine 등)")]
    public string subType = "AR";
    public NoiseLevel noise = NoiseLevel.Normal;

    // ===== Core Combat Stats (기본값) =====
    [Header("Core (Base)")]
    [Min(0)] public int damage = 10;                   // 데미지
    [Min(0)] public int rpm = 360;                     // 분당 발사 속도 (RPM)
    [Min(1)] public int curMagazine = 0;               // 탄창 용량
    [Min(0)] public int curReserve = 0;               // 예비 탄약
    [Min(1)] public int maxMagazine = 0;               // 전체 탄창 용량
    [Min(0)] public int maxReserve = 0;               // 전체 예비 탄약
    [Tooltip("정확도(°). 값이 낮을수록 정밀함 (스크린샷의 5.5° 같은 수치).")]
    [Range(0f, 30f)] public float accuracyDeg = 6f;
    [Min(1)] public int projectilesPerShot = 1;        // 샷건/버스트 지원
    [Min(0.1f)] public float bulletSpeed = 20f;
    [Min(0.05f)] public float bulletLife = 2f;

    // ===== Advanced (스크린샷의 추가 항목들 매핑) =====
    [Header("Advanced (Base)")]
    [Tooltip("단발 지연(샷 후 회복/딜레이). 0 = 없음")]
    [Range(0f, 50f)] public float semiAutoDelay = 0f;  // 단발 지연 (예: 12.8)
    [Tooltip("탄퍼짐/반동 제어(%) — +10%면 제어 개선")]
    public float spreadControlPct = 0f;                // 탄퍼짐 제어 (%)
    [Tooltip("조준 거리(%) — 100% 기본, 110%면 10% 증가")]
    public float aimDistancePct = 100f;                // 조준거리 (%)

    [Header("Handling (Base)")]
    [Tooltip("재장전 속도 배수(1.00 = 기본, 0.88 = 12% 빨라짐)")]
    public float reloadSpeedMul = 1.00f;               // 재장전 속도
    [Tooltip("ADS(정조준) 속도(%). 100% 기본, 98%는 2% 느림")]
    public float adsSpeedPct = 100f;                   // 정조준 속도
    [Tooltip("이동 속도 페널티(%). 6%면 이동속도 6% 감소")]
    public float movePenaltyPct = 0f;                  // 이동 감소

    // ===== Recoil (기존 Shooter 호환) =====
    [Header("Recoil (Shooter Compatibility)")]
    [Tooltip("발사당 반동 증가량(내부 계산)")]
    [Min(0f)] public float recoilAmount = 0f;
    [Tooltip("반동 초당 회복량")]
    [Min(0f)] public float recoilRecovery = 0f;

    // ===== Visuals / Anim / Prefabs =====
    [Header("Assets")]
    public GameObject bulletPrefab;                    // Rigidbody2D + Collider2D(isTrigger)
    public GameObject muzzleFlashPrefab;               // 옵션
    public RuntimeAnimatorController upperAnimator;    // 상체 애니메이터
    public Sprite weaponSprite;                        // 무기 이미지
    public Vector2 firePointOffset;                    // 발사 위치 오프셋

    // ===== Attachments (스크린샷 헤더 참고) =====
    [Header("Attachments (Names Only)")]
    public string scopeName;
    public string flashlightName;
    public string laserName;
    public string gripName;
    public string magazineName;
    public string compensatorName;

    // ---- Derived helpers ----
    /// <summary>초당 발사 수(=RPM/60). Shooter가 기존 fireRate를 사용한다면 이 값을 전달하세요.</summary>
    public float FireRatePerSec => Mathf.Max(0.1f, rpm / 60f);

#if UNITY_EDITOR
private void OnValidate()
{
    // 상한은 0 미만 불가
    rpm         = Mathf.Max(0, rpm);
    maxMagazine = Mathf.Max(1, maxMagazine);
    maxReserve  = Mathf.Max(0, maxReserve);

    // Cur 값은 Max 상한으로 클램프 (지금처럼 Max로 덮어쓰지 말고!)
    curMagazine = Mathf.Clamp(curMagazine, 0, maxMagazine);
    curReserve  = Mathf.Clamp(curReserve,  0, maxReserve);

    bulletSpeed   = Mathf.Max(0.1f, bulletSpeed);
    bulletLife    = Mathf.Max(0.05f, bulletLife);
    accuracyDeg   = Mathf.Clamp(accuracyDeg, 0f, 30f);
    reloadSpeedMul= Mathf.Max(0.1f, reloadSpeedMul);
    adsSpeedPct   = Mathf.Max(1f,    adsSpeedPct);
}
#endif
}
