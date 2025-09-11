using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform gunPoint;   // 총구 위치(자식 Transform)
    public GunData gun;          // SO로 총 스펙 지정

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
        if (gun == null || gunPoint == null || gun.bulletPrefab == null) return false;
        if (respectFireRate && cooldown > 0f) return false;

        // 발사
        int count = Mathf.Max(1, gun.projectilesPerShot);
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = ApplySpread(direction.normalized, gun.spread);
            SpawnBullet(dir);
        }

        // 쿨다운 설정
        if (respectFireRate)
            cooldown = 1f / Mathf.Max(0.001f, gun.fireRate);

        //VFX / SFX
        if (gun.muzzleFlashPrefab)
        {
            var fx = Instantiate(gun.muzzleFlashPrefab, gunPoint.position, gunPoint.rotation);
            Destroy(fx, 0.05f);
        }
        if (gun.shotSfx)
        {
            AudioSource.PlayClipAtPoint(gun.shotSfx, gunPoint.position);
        }

        return true;
    }

    private void SpawnBullet(Vector2 dir)
    {
        var go = Instantiate(gun.bulletPrefab);
        var b = go.GetComponent<Bullet>();
        if (!b)
        {
            Debug.LogError("Bullet 프리팹에 Bullet2D 컴포넌트가 필요합니다.");
            Destroy(go);
            return;
        }

        b.Init(
            position: gunPoint.position,
            dir: dir,
            speed: gun.bulletSpeed,
            damage: gun.damage,
            lifetime: gun.bulletLife,
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
