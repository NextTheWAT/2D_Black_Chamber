using UnityEngine;
using Constants;

public class Shooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform gunPoint; // 현재 무기 총구 (WeaponManager에서 세팅)
    public GunData gunData;    // 현재 무기 데이터

    // ---- Ammo State (런타임 상태; 시작값은 GunData로부터) ----
    private int currentMagazine; // 현재 탄창
    private int currentAmmo;     // 현재 예비 탄
    private float currentSpread;  // 현재 반동/퍼짐

    // 외부는 조회만 가능하게 (수정은 WeaponManager 경유)
    public int CurrentMagazine => currentMagazine;
    public int CurrentAmmo => currentAmmo;

    [Header("Options")]
    public bool respectFireRate = true;
    public bool shooterLocked = false; // 발사 금지

    // 무기별 쿨다운을 별도로 관리 (전환 시 쿨다운 꼬임 방지)
    private float cooldown;

    public bool HasAnyAmmo => (currentMagazine + currentAmmo) > 0;
    public bool IsClipEmpty => currentMagazine <= 0;
    private bool _initialized = false;

    public float CurrentSpread
    {
        get => currentSpread;
        // 기존 gunData.spread -> gunData.accuracyDeg (기본 확산 각도)
        set => currentSpread = Mathf.Clamp(value, 0f, gunData ? gunData.accuracyDeg : 0f);
    }

    private void Awake()
    {
        if (gunPoint == null)
        {
            gunPoint = new GameObject("GunPoint").transform;
            gunPoint.SetParent(transform);
            gunPoint.localPosition = Vector3.zero;
            gunPoint.localRotation = Quaternion.identity;
        }
    }

    private void Start()
    {
        // WeaponManager가 Initialize를 호출하지 않는 적 프리팹 등을 위해 안전장치
        if (!_initialized && gunData != null)
        {
            Initialize(gunData);
        }
    }

    private void Update()
    {
        if (!respectFireRate) return;
        if (cooldown > 0f) cooldown -= Time.deltaTime;

        RecoverSpread(); // 반동 회복
    }

    public void RecoverSpread()
    {
        if (gunData == null) return;

        // 반동 회복
        CurrentSpread -= gunData.recoilRecovery * Time.deltaTime;
    }

    public void Initialize(GunData gunData)
    {
        this.gunData = gunData;

        // 시작 탄약은 Cur에서 읽고, 상한은 Max로 클램프
        currentMagazine = Mathf.Clamp(gunData.curMagazine, 0, gunData.maxMagazine);
        currentAmmo = Mathf.Clamp(gunData.curReserve, 0, gunData.maxReserve);

        gunPoint.SetLocalPositionAndRotation(gunData.firePointOffset, Quaternion.identity);
        cooldown = 0f;
        _initialized = true;

        WeaponManager.Instance?.OnAmmoChanged?.Invoke();
    }


    /// <summary>현재 무기 타입에 맞춰 발사</summary>
    public bool Shoot()
        => Shoot(gunPoint.up);

    public bool Shoot(Vector2 direction)
    {
        // 필수 레퍼런스 검사
        if (gunData == null || gunPoint == null || gunData.bulletPrefab == null) return false;

        // 발사 간격 체크
        if (respectFireRate && cooldown > 0f) return false;

        // 탄약 소비
        if (currentMagazine <= 0 || shooterLocked)
        {
            // 빈 탄창: 틱틱 사운드만
            WeaponSoundManager.Instance?.PlayEmptySound();
            if (respectFireRate)
                cooldown = 1f / Mathf.Max(0.001f, gunData.FireRatePerSec); // 틱틱 템포 유지(선택)
            return false;
        }

        currentMagazine = Mathf.Max(0, currentMagazine - 1);
        WeaponManager.Instance?.OnAmmoChanged?.Invoke();

        // 발사 처리
        int count = Mathf.Max(1, gunData.projectilesPerShot);
        Vector2 baseDir = direction.sqrMagnitude > 0.0001f ? direction.normalized
                                                           : (Vector2)gunPoint.right;

        for (int i = 0; i < count; i++)
        {
            Vector2 dir = ApplySpread(baseDir, CurrentSpread);
            SpawnBullet(gunData, gunPoint, dir);
        }

        // 반동 증가
        CurrentSpread += gunData.recoilAmount;

        // 다음 발사까지 쿨다운
        if (respectFireRate)
            cooldown = 1f / Mathf.Max(0.001f, gunData.FireRatePerSec);

        // 이펙트
        if (gunData.muzzleFlashPrefab)
        {
            var fx = Instantiate(gunData.muzzleFlashPrefab, gunPoint.position, gunPoint.rotation);
            Destroy(fx, 0.05f);
        }

        // 사운드 (간단한 예)
        if (gunData.displayName != null)
        {
            if (gunData.displayName.Contains("Pistol"))
                WeaponSoundManager.Instance?.PlayPistolShootSound();
            else if (gunData.displayName.Contains("Rifle"))
                WeaponSoundManager.Instance?.PlayRifleShootSound();
        }

        return true;
    }

    public bool Reload()
    {
        if (gunData == null) return false;

        // 1) 현재 탄창이 이미 가득이면 종료
        if (currentMagazine >= gunData.maxMagazine) return false;

        // 2) 리저브(오른쪽)가 없으면 종료
        if (currentAmmo <= 0) return false;

        // 3) 채워야 할 탄수 = (탄창 용량 - 현재 탄창 탄수)
        int need = gunData.maxMagazine - currentMagazine;
        if (need <= 0) return false;

        // 4) 실제로 옮길 수 있는 탄수 = min(need, 리저브)
        int move = Mathf.Min(need, currentAmmo);

        // 5) 오른쪽(리저브) → 왼쪽(탄창)으로 이동
        currentMagazine += move;   // 탄창 채움
        currentAmmo -= move;       // 리저브 감소

        cooldown = 0.5f; // 필요 시 데이터화

        WeaponManager.Instance?.OnAmmoChanged?.Invoke();
        WeaponManager.Instance?.OnReloaded?.Invoke();
        WeaponSoundManager.Instance?.PlayReloadSound();

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

    // === Ammo write APIs: 외부에서 직접 수정 못 하게 WeaponManager만 경유하도록 internal ===

    internal int AddAmmo(int amount)
    {
        if (gunData == null || amount <= 0) return 0;
        int cap = Mathf.Max(0, gunData.maxReserve);
        int before = currentAmmo;
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, cap);
        int gained = currentAmmo - before;
        if (gained != 0) WeaponManager.Instance?.OnAmmoChanged?.Invoke();
        return gained;
    }

    // 무기 상태 복원/세이브 용도로만 내부 사용
    internal void ForceSetAmmo(int magazine, int reserve)
    {
        int cap = gunData != null ? Mathf.Max(0, gunData.maxReserve) : 0;
        currentMagazine = Mathf.Clamp(magazine, 0, gunData != null ? gunData.maxMagazine : 0);
        currentAmmo = Mathf.Clamp(reserve, 0, cap);
        WeaponManager.Instance?.OnAmmoChanged?.Invoke();
    }
}
