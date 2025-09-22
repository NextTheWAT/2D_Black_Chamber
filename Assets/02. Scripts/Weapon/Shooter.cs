using UnityEngine;
using Constants;

public class Shooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform gunPistolPoint;  // 피스톨 총구
    public Transform gunRiflePoint;   // 라이플 총구
    public GunData pistol;            // 피스톨 스펙
    public GunData rifle;             // 라이플 스펙

    [Header("Options")]
    public bool respectFireRate = true;

    // 무기별 쿨다운을 별도로 관리 (전환 시 쿨다운 꼬임 방지)
    private float cooldownPistol;
    private float cooldownRifle;

    private void Update()
    {
        if (!respectFireRate) return;
        if (cooldownPistol > 0f) cooldownPistol -= Time.deltaTime;
        if (cooldownRifle > 0f) cooldownRifle -= Time.deltaTime;
    }

    /// <summary>현재 무기 타입에 맞춰 발사</summary>
    public bool Shoot(Vector2 direction)
    {
        var wm = WeaponManager.Instance;
        var type = wm ? wm.CurrentWeapon : WeaponType.Pistol;

        // 현재 무기에 맞는 데이터/총구/쿨다운 선택
        GunData gun;
        Transform muzzle;
        ref float cd = ref cooldownPistol;

        if (type == WeaponType.Rifle)
        {
            gun = rifle;
            muzzle = gunRiflePoint;
            cd = ref cooldownRifle;
        }
        else
        {
            gun = pistol;
            muzzle = gunPistolPoint;
            cd = ref cooldownPistol;
        }

        // 필수 레퍼런스 검사
        if (gun == null || muzzle == null || gun.bulletPrefab == null) return false;

        // 발사 간격 체크
        if (respectFireRate && cd > 0f) return false;

        // 탄약 소비 (현재 무기 기준으로 BulletManager가 알아서 처리)
        if (!BulletManager.Instance.TryConsumeOneBullet())
        {
            // 빈 탄창: 틱틱 사운드만
            WeaponSoundManager.Instance?.PlayEmptySound();
            if (respectFireRate)
                cd = 1f / Mathf.Max(0.001f, gun.fireRate); // 틱틱 템포 유지(선택)
            return false;
        }

        // 발사 처리
        int count = Mathf.Max(1, gun.projectilesPerShot);
        Vector2 baseDir = direction.sqrMagnitude > 0.0001f ? direction.normalized
                                                           : (Vector2)muzzle.right;

        for (int i = 0; i < count; i++)
        {
            Vector2 dir = ApplySpread(baseDir, gun.spread);
            SpawnBullet(gun, muzzle, dir);
        }

        // 다음 발사까지 쿨다운
        if (respectFireRate)
            cd = 1f / Mathf.Max(0.001f, gun.fireRate);

        // 이펙트/사운드
        if (gun.muzzleFlashPrefab)
        {
            var fx = Instantiate(gun.muzzleFlashPrefab, muzzle.position, muzzle.rotation);
            Destroy(fx, 0.05f);
        }

        if (type == WeaponType.Rifle)
            WeaponSoundManager.Instance?.PlayRifleShootSound();
        else
            WeaponSoundManager.Instance?.PlayPistolShootSound();

        return true;
    }

    private void SpawnBullet(GunData gun, Transform muzzle, Vector2 dir)
    {
        var go = Instantiate(gun.bulletPrefab);
        var b = go.GetComponent<Bullet>();
        if (!b)
        {
            Debug.LogError("Bullet 프리팹에 Bullet 컴포넌트가 필요합니다.", go);
            Destroy(go);
            return;
        }

        b.Init(
            position: muzzle.position,
            dir: dir,
            speed: gun.bulletSpeed,
            damage: gun.damage,
            lifetime: gun.bulletLife,
            ignoreLayer: gameObject.layer
        );
    }

    private static Vector2 ApplySpread(Vector2 baseDir, float spreadDeg)
    {
        if (spreadDeg <= 0f) return baseDir;
        float half = spreadDeg * 0.5f;
        float rad = Random.Range(-half, half) * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        return new Vector2(baseDir.x * c - baseDir.y * s,
                           baseDir.x * s + baseDir.y * c).normalized;
    }
}
