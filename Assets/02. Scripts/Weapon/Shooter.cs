using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform gunPistonPoint;   // 총구 위치(자식 Transform)
    public Transform gunRiflePoint;
    public GunData pistol;          // SO로 총 스펙 지정
    public GunData rifle;

    [Header("Options")]
    public bool respectFireRate = true; // true면 fireRate(초당 발사수) 준수

    private float cooldown;

    private void Update()
    {
        if (!respectFireRate) return;
        if (cooldown > 0f) cooldown -= Time.deltaTime;
    }

    /// <summary>
    /// 방향 벡터 기준으로 발사. 성공 시 true, 실패(쿨다운/세팅누락) 시 false
    /// </summary>
    public bool Shoot(Vector2 direction)
    {
        if (gun == null || gunPistonPoint == null || pistol.bulletPrefab == null) return false;
        if (respectFireRate && cooldown > 0f) return false;

        // 1) 탄약 소비 시도 (탄창에 없으면 false)
        if (!BulletManager.Instance.TryConsumeOneBullet())
        {
            // 빈 탄창: 틱틱 사운드만
            WeaponSoundManager.Instance.PlayEmptySound(); // 사운드 매니저에 이 메서드 하나만 추가해줘
            if (respectFireRate)
                cooldown = 1f / Mathf.Max(0.001f, pistol.fireRate); // 연사속도에 맞춰 틱틱 템포 유지(선택)
            return false;
        }

        // 2) 발사 처리
        int count = Mathf.Max(1, pistol.projectilesPerShot);
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = ApplySpread(direction.normalized, pistol.spread);
            SpawnBullet(dir);
        }

        if (respectFireRate)
            cooldown = 1f / Mathf.Max(0.001f, pistol.fireRate);

        if (pistol.muzzleFlashPrefab)
        {
            var fx = Instantiate(pistol.muzzleFlashPrefab, gunPistonPoint.position, gunPistonPoint.rotation);
            Destroy(fx, 0.05f);
        }

        WeaponSoundManager.Instance.PlayPistolShootSound(); // 필요하면 무기 타입에 따라 분기

        return true;
    }

    private void SpawnBullet(Vector2 dir)
    {
        var go = Instantiate(pistol.bulletPrefab);
        var b = go.GetComponent<Bullet>();
        if (!b)
        {
            Debug.LogError("Bullet 프리팹에 Bullet2D 컴포넌트가 필요합니다.");
            Destroy(go);
            return;
        }

        b.Init(
            position: gunPistonPoint.position,
            dir: dir,
            speed: pistol.bulletSpeed,
            damage: pistol.damage,
            lifetime: pistol.bulletLife,
            ignoreLayer: gameObject.layer  // 자기 레이어는 무시
        );
    }

    private static Vector2 ApplySpread(Vector2 baseDir, float spreadDeg)
    {
        if (spreadDeg <= 0f) return baseDir;
        float half = spreadDeg * 0.5f;
        float rad = Random.Range(-half, half) * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        return new Vector2(baseDir.x * c - baseDir.y * s, baseDir.x * s + baseDir.y * c).normalized;
    }
}
