using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform gunPoint;   // �ѱ� ��ġ(�ڽ� Transform)
    public GunData gun;          // SO�� �� ���� ����

    [Header("Options")]
    public bool respectFireRate = true; // true�� fireRate(�ʴ� �߻��) �ؼ�

    private float cooldown;

    private void Update()
    {
        if (!respectFireRate) return;
        if (cooldown > 0f) cooldown -= Time.deltaTime;
    }

    /// <summary>
    /// ���� ���� �������� �߻�. ���� �� true, ����(��ٿ�/���ô���) �� false
    /// </summary>
    public bool Shoot(Vector2 direction)
    {
        if (gun == null || gunPoint == null || gun.bulletPrefab == null) return false;
        if (respectFireRate && cooldown > 0f) return false;

        // �߻�
        int count = Mathf.Max(1, gun.projectilesPerShot);
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = ApplySpread(direction.normalized, gun.spread);
            SpawnBullet(dir);
        }

        // ��ٿ� ����
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
            Debug.LogError("Bullet �����տ� Bullet2D ������Ʈ�� �ʿ��մϴ�.");
            Destroy(go);
            return;
        }

        b.Init(
            position: gunPoint.position,
            dir: dir,
            speed: gun.bulletSpeed,
            damage: gun.damage,
            lifetime: gun.bulletLife,
            ignoreLayer: gameObject.layer  // �ڱ� ���̾�� ����
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
