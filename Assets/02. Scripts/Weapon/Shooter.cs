using UnityEngine;
using Constants;

public class Shooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform gunPoint; // 현재 무기 총구 (WeaponManager에서 세팅)
    public GunData gunData; // 현재 무기 데이터
    private int currentMagazine; // 현재 탄창
    private int currentAmmo;  // 현재 탄약

    public int CurrentMagazine => currentMagazine;
    public int CurrentAmmo => currentAmmo;

    [Header("Options")]
    public bool respectFireRate = true;

    // 무기별 쿨다운을 별도로 관리 (전환 시 쿨다운 꼬임 방지)
    private float cooldown;

    private void Awake()
    {
        gunPoint = new GameObject("GunPoint").transform;
        gunPoint.SetParent(transform);
    }

    private void Start()
    {
        Initialize(gunData);
    }

    private void Update()
    {
        if (!respectFireRate) return;
        if (cooldown > 0f) cooldown -= Time.deltaTime;
    }

    public void Initialize(GunData gunData)
    {
        this.gunData = gunData;
        currentMagazine = gunData.maxMagazine;
        currentAmmo = gunData.maxAmmo;
        gunPoint.localPosition = gunData.firePointOffset;
        cooldown = 0f;
        WeaponManager.Instance.OnAmmoChanged?.Invoke();
    }

    /// <summary>현재 무기 타입에 맞춰 발사</summary>
    public bool Shoot(Vector2 direction)
    {
        // 필수 레퍼런스 검사
        if (gunData == null || gunPoint == null || gunData.bulletPrefab == null) return false;

        // 발사 간격 체크
        if (respectFireRate && cooldown > 0f) return false;

        // 탄약 소비
        if (currentAmmo <= 0)
        {
            // 빈 탄창: 틱틱 사운드만
            WeaponSoundManager.Instance.PlayEmptySound();
            if (respectFireRate)
                cooldown = 1f / Mathf.Max(0.001f, gunData.fireRate); // 틱틱 템포 유지(선택)
            return false;
        }

        currentAmmo--;
        WeaponManager.Instance.OnAmmoChanged?.Invoke();

        // 발사 처리
        int count = Mathf.Max(1, gunData.projectilesPerShot);
        Vector2 baseDir = direction.sqrMagnitude > 0.0001f ? direction.normalized
                                                           : (Vector2)gunPoint.right;

        for (int i = 0; i < count; i++)
        {
            Vector2 dir = ApplySpread(baseDir, gunData.spread);
            SpawnBullet(gunData, gunPoint, dir);
        }

        // 다음 발사까지 쿨다운
        if (respectFireRate)
            cooldown = 1f / Mathf.Max(0.001f, gunData.fireRate);

        // 이펙트
        if (gunData.muzzleFlashPrefab)
        {
            var fx = Instantiate(gunData.muzzleFlashPrefab, gunPoint.position, gunPoint.rotation);
            Destroy(fx, 0.05f);
        }

        // 사운드
        if (gunData.displayName.Contains("Pistol"))
            WeaponSoundManager.Instance.PlayPistolShootSound();
        else if(gunData.displayName.Contains("Rifle"))
            WeaponSoundManager.Instance.PlayRifleShootSound();

        return true;
    }

    public void Reload()
    {
        if (gunData == null) return;
        if (currentAmmo >= gunData.maxAmmo) return; // 이미 가득
        if (currentMagazine <= 0) return; 

        currentMagazine--;
        currentAmmo = gunData.maxAmmo;
        cooldown = 0.5f; // 리로드 시간 (임시)

        WeaponManager.Instance.OnAmmoChanged?.Invoke();
        WeaponManager.Instance.OnReloaded?.Invoke();

        // 리로드 사운드
        WeaponSoundManager.Instance.PlayReloadSound();
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
            noiseRange: gun.noiseRange,
            lifetime: gun.bulletLife,
            ignoreLayer: gameObject.layer,
            bulletTag: gameObject.tag
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
