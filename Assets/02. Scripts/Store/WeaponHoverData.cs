using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponHoverData
{
    public string weaponName;

    [Header("부착물 이름")]
    public string scopeName;        // 스코프
    public string flashlightName;   // 플래시라이트
    public string laserName;        // 레이저포인트
    public string gripName;         // 손잡이
    public string magazineName;     // 탄창
    public string compensatorName;  // 보정기

    [Header("스탯 정보")]
    public string category;         // 분류
    public float damage;            // 데미지
    public float fireRate;          // 발사속도
    public int ammoCapacity;        // 장탄 수
    public float accuracy;          // 정확도
    public float noise;             // 소음 수치
    public float spread;            // 탄 퍼짐
    public float recoilControl;     // 반동제어
    public float aimRange;          // 조준 사거리
    public float accuracyRecovery;  // 조준 안정성
    public float bulletSpeed;       // 탄속
    public float reloadSpeed;       // 장전속도
    public float mobilityReduction; // 이동 감소
}
