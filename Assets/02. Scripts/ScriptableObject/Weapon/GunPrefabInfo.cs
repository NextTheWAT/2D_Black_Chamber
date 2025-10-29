using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunPrefabInfo", menuName = "ScriptableObjects/Weapon/GunPrefabInfo")]
public class GunPrefabInfo : ScriptableObject
{
    [Header("Loadout Tag / Shop Flags")]
    public GunData.PhaseTag phaseTag = GunData.PhaseTag.Any;   // 스텔스/난전 자동 선택에 사용
    public bool autoOwnedOnStart = false;      // 시작 소유 여부
    public bool hideFromShop = false;          // 상점에서 숨김
    
    [Header("Assets / Prefabs")]
    public GameObject bulletPrefab;                    // Rigidbody2D+Collider2D(isTrigger)
    public GameObject muzzleFlashPrefab;
    public RuntimeAnimatorController upperAnimator;
    public Sprite weaponSprite;
    public Vector2 firePointOffset;
}
