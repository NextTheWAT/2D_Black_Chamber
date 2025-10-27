using UnityEngine;
using UnityEngine.Serialization; // FormerlySerializedAs
using System;

/// <summary>
/// Weapon ScriptableObject (Shop/Armory/Runtime 공통 데이터)
/// 스펙: 작성자가 제공한 표(스크린샷) 기준으로 설계됨.
/// - 저장/표시는 여기 정의된 필드명 사용
/// - 기존 필드와의 호환을 위해 FormerlySerializedAs를 적극 사용 (에셋 값 유지)
/// </summary>
[CreateAssetMenu(menuName = "Gun/Gun Data")]
public class GunData : ScriptableObject
{
    // ============= 공통 태그 (기존 시스템 호환) =============
    public enum PhaseTag { Any, Stealth, Combat } // 전투/잠입 로드아웃 구분(기존 무기매니저 호환)
    [Header("Loadout Tag / Shop Flags")]
    public PhaseTag phaseTag = PhaseTag.Any;   // 스텔스/난전 자동 선택에 사용
    public bool autoOwnedOnStart = false;      // 시작 소유 여부
    public bool hideFromShop = false;          // 상점에서 숨김

    // ============= ID / 표시 정보 / 분류 =============
    [Header("Identity")]
    public int id = 0;                         // 무기 고유 ID (표 스펙)
    [FormerlySerializedAs("displayName")]
    public string weaponName = "Pistol";       // 무기 표시 이름 (예: "M1911")
    public int Grade = 4;                      // 무기 등급(1~4 등, 스펙 표의 4~1 역순 사용 가능)

    // 분류: HG/SMG/AR/SG/SR (스펙 표)
    public enum WeaponSubType { HG, SMG, AR, SG, SR }
    [Header("Classification")]
    public WeaponSubType subType = WeaponSubType.HG;

    [Header("Attachment (IDs as CSV)")]
    public string attachment = "";             // 장착 가능 부착물 ID 목록 (콤마로 구분)

    // ============= 상점 =============
    [Header("Shop")]
    [Min(0)] public int price = 0;             // 상점 판매 가격

    // ============= 전투 기본 수치 =============
    [Header("Core Damage / Firing")]
    [Min(0)] public int damage = 10;          // 데미지
    [Min(0)] public int rpm = 360;            // 분당 발사 속도 (Rounds Per Minute)
    [Range(0.1f, 500f)] public float bulletSpeed = 25f; // 탄속 (m/s)
    [FormerlySerializedAs("projectilesPerShot")]
    [Min(1)] public int bulletPerShot = 1;     // 발당 발사 총알 수(샷건 등)
    [FormerlySerializedAs("bulletLife")]
    [Range(0.01f, 30f)] public float bulletDuration = 2f; // 탄이 소멸되기까지의 시간(초)
    public NoiseData gunNoiseData; // 발사 소음 데이터

    [Header("Ammo (Magazine / Reserve)")]
    [FormerlySerializedAs("curMagazine")]
    [Min(0)] public int curAmmo = 0;           // 시작/현재 장전 탄
    [FormerlySerializedAs("curReserve")]
    [Min(0)] public int curReserveAmmo = 0;    // 시작/현재 예비 탄
    [FormerlySerializedAs("maxMagazine")]
    [Min(1)] public int maxAmmo = 12;          // 최대 장전 탄
    [FormerlySerializedAs("maxReserve")]
    [Min(0)] public int maxReserveAmmo = 60;   // 최대 예비 탄

    // ============= 사운드/제어/정확도 =============
    [Header("Sound / Control")]
    [Range(0f, 100f)] public float volume = 50f;       // 발포음(가청도) 0~100
    // ※ 아래 accuracy/precision/stability는 표 의미 그대로 사용
    [Tooltip("조준선의 길이 (높을수록 길어짐)")]
    public float accuracy = 5.5f;
    [Tooltip("조준선이 양복 운동하는 범위(0~100), 높을수록 탄퍼짐 증가")]
    [Range(0f, 100f)] public float precision = 20f;
    [Tooltip("조준선이 양복 운동하는 속도(2~10), 높을수록 더 느리게")]
    [Range(2f, 10f)] public float stability = 5f;

    [Header("ADS (정조준 특성)")]
    [Tooltip("조준 시 조준선의 길이 (높을수록 길어짐)")]
    public float aimAccuracy = 5.0f;
    [Tooltip("조준 시 조준선의 양복 범위(0~100)")]
    [Range(0f, 100f)] public float aimPrecision = 15f;
    [Tooltip("조준 시 조준선의 양복 속도(2~10)")]
    [Range(2f, 10f)] public float aimStability = 4.5f;

    [Header("Aim / Movement")]
    [Tooltip("플레이어가 볼 수 있는 최대 거리(기본 100%). 높을수록 멀리 보임")]
    public float aimDistance = 100f;           // %
    public bool hasLaser = false;              // 레이저 표시 여부

    // ============= 재장전 / 모션 =============
    [Header("Reload / Handling")]
    [Tooltip("기본 재장전 시간(초)")]
    public float baseReloadTime = 2.0f;
    [FormerlySerializedAs("reloadSpeedMul")]
    [Tooltip("재장전 속도 배율(100 = 기본 속도, 120 = 20% 빨라짐)")]
    public float reloadSpeed = 100f;           // %
    [FormerlySerializedAs("movePenaltyPct")]
    [Tooltip("이동 속도 배율(%). 100 = 기본, 95 = 5% 감소")]
    public float moveSpeedModifier = 100f;     // %
    [FormerlySerializedAs("adsSpeedPct")]
    [Tooltip("ADS 상태 추가 이동 속도 배율(%). 최종 = (moveSpeedModifier/100) * (aimMoveSpeedModifier/100)")]
    public float aimMoveSpeedModifier = 100f;  // %

    // ============= 리코일(기존 Shooter 호환용) =============
    [Header("Recoil (Legacy Compatibility)")]
    [Min(0f)] public float recoilAmount = 0f;
    [Min(0f)] public float recoilRecovery = 0f;

    // ============= 비주얼/프리팹 =============
    [Header("Assets / Prefabs")]
    public GameObject bulletPrefab;                    // Rigidbody2D+Collider2D(isTrigger)
    public GameObject muzzleFlashPrefab;
    public RuntimeAnimatorController upperAnimator;
    public Sprite weaponSprite;
    public Vector2 firePointOffset;

    // ============= 유틸/파생 속성 =============
    /// <summary>초당 발사 수 (= RPM / 60)</summary>
    public float FireRatePerSec => Mathf.Max(0.01f, rpm / 60f);

    /// <summary>표시용 이름. 기존 displayName 대체</summary>
    public string DisplayName => string.IsNullOrEmpty(weaponName) ? name : weaponName;

    /// <summary>로드아웃/세이브 키: id가 있으면 id, 없으면 ScriptableObject name</summary>
    public string Key => id > 0 ? id.ToString() : name;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 상/하한 클램프
        rpm = Mathf.Max(0, rpm);
        damage = Mathf.Max(0, damage);
        bulletSpeed = Mathf.Clamp(bulletSpeed, 0.1f, 500f);
        bulletDuration = Mathf.Clamp(bulletDuration, 0.01f, 30f);

        maxAmmo = Mathf.Max(1, maxAmmo);
        maxReserveAmmo = Mathf.Max(0, maxReserveAmmo);
        curAmmo = Mathf.Clamp(curAmmo, 0, maxAmmo);
        curReserveAmmo = Mathf.Clamp(curReserveAmmo, 0, maxReserveAmmo);

        volume = Mathf.Clamp(volume, 0f, 100f);
        precision = Mathf.Clamp(precision, 0f, 100f);
        stability = Mathf.Clamp(stability, 2f, 10f);
        aimPrecision = Mathf.Clamp(aimPrecision, 0f, 100f);
        aimStability = Mathf.Clamp(aimStability, 2f, 10f);

        reloadSpeed = Mathf.Max(1f, reloadSpeed);         // %
        moveSpeedModifier = Mathf.Max(1f, moveSpeedModifier);   // %
        aimMoveSpeedModifier = Mathf.Max(1f, aimMoveSpeedModifier);// %

        // 에디터에서 표시 이름 비어있으면 SO 이름로
        if (string.IsNullOrEmpty(weaponName)) weaponName = name;
    }
#endif
}
