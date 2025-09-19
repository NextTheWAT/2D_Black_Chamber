using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Gun Data")]
public class GunData : ScriptableObject
{
    [Header("Stats")]
    public string displayName = "Pistol";
    [Min(0f)] public int damage = 10;
    [Min(0f)] public float bulletSpeed = 20f;
    [Tooltip("Shots per second")][Min(0.1f)] public float fireRate = 6f;
    [Range(0f, 20f)] public float spread = 0f;        // 도(°)
    [Min(1)] public int projectilesPerShot = 1;        // 샷건/버스트도 지원, 기본 1발

    [Header("Assets")]
    public GameObject bulletPrefab;                    // Rigidbody2D + Collider2D(isTrigger)
    public GameObject muzzleFlashPrefab;               // 옵션

    [Header("Bullet Lifetime")]
    [Min(0.1f)] public float bulletLife = 2f;
}
