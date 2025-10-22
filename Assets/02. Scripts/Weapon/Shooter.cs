using UnityEngine;
using Constants;
using System.Reflection;

public class Shooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform gunPoint; // 현재 무기 총구 (WeaponManager에서 세팅)
    public GunData gunData;    // 현재 무기 데이터

    // ---- Ammo State (런타임 상태; 시작값은 GunData로부터) ----
    private int currentMagazine; // 현재 탄창
    private int currentAmmo;     // 현재 예비 탄
    private float currentSpread; // 현재 반동/퍼짐

    // 외부는 조회만 가능하게 (수정은 WeaponManager 경유)
    public int CurrentMagazine => currentMagazine;
    public int CurrentAmmo => currentAmmo;

    [Header("Options")]
    public bool respectFireRate = true;
    public bool shooterLocked = false; // 발사 금지

    private float cooldown;
    private bool _initialized = false;

    // 캐시(신/구 스펙 호환 계산 결과)
    private int capMagazine;
    private int capReserve;
    private float projLife;
    private int projPerShot;
    private float baseSpreadDeg;
    private float fireRatePerSec; // = rpm/60

    public bool HasAnyAmmo => (currentMagazine + currentAmmo) > 0;
    public bool IsClipEmpty => currentMagazine <= 0;

    public float CurrentSpread
    {
        get => currentSpread;
        set => currentSpread = Mathf.Clamp(value, 0f, baseSpreadDeg);
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
        if (!_initialized && gunData != null)
            Initialize(gunData);
    }

    private void Update()
    {
        if (!respectFireRate) return;
        if (cooldown > 0f) cooldown -= Time.deltaTime;
        RecoverSpread();
    }

    public void RecoverSpread()
    {
        if (gunData == null) return;
        float recover = GetFloat(gunData, 0f, "recoilRecovery", "recoilControl");
        CurrentSpread -= recover * Time.deltaTime;
    }

    public void Initialize(GunData gd)
    {
        gunData = gd;

        // ---- 스펙 매핑 & 캐시 ----
        capMagazine = GetInt(gd, 12, "maxAmmo", "maxMagazine");
        capReserve = GetInt(gd, 60, "maxReserveAmmo", "maxReserve");

        int startMag = Mathf.Clamp(GetInt(gd, 0, "curAmmo", "curMagazine"), 0, capMagazine);
        int startReserve = Mathf.Clamp(GetInt(gd, 0, "curReserveAmmo", "curReserve"), 0, capReserve);

        projLife = GetFloat(gd, 2f, "bulletDuration", "bulletLife");
        projPerShot = Mathf.Max(1, GetInt(gd, 1, "bulletPerShot", "projectilesPerShot"));

        // spread 기준: accuracyDeg(신) 없으면 precision(0~100)을 각도로 가정
        baseSpreadDeg = GetFloat(gd, 0f, "accuracyDeg", "precision");

        int rpm = GetInt(gd, 360, "rpm", "fireRate");
        fireRatePerSec = Mathf.Max(0.01f, rpm / 60f);

        currentMagazine = startMag;
        currentAmmo = startReserve;

        // 총구 위치 (있다면)
        var fpo = GetVector2(gd, "firePointOffset");
        if (fpo.HasValue)
            gunPoint.SetLocalPositionAndRotation(fpo.Value, Quaternion.identity);

        cooldown = 0f;
        _initialized = true;

        WeaponManager.Instance?.OnAmmoChanged?.Invoke();
    }

    public bool Shoot() => Shoot(gunPoint.up);

    public bool Shoot(Vector2 direction)
    {
        if (gunData == null || gunPoint == null || gunData.bulletPrefab == null) return false;
        if (respectFireRate && cooldown > 0f) return false;

        if (currentMagazine <= 0 || shooterLocked)
        {
            WeaponSoundManager.Instance?.PlayEmptySound(transform.position);
            if (respectFireRate) cooldown = 1f / fireRatePerSec;
            return false;
        }

        currentMagazine = Mathf.Max(0, currentMagazine - 1);
        WeaponManager.Instance?.OnAmmoChanged?.Invoke();

        int count = projPerShot;
        Vector2 baseDir = direction.sqrMagnitude > 0.0001f ? direction.normalized : (Vector2)gunPoint.right;

        for (int i = 0; i < count; i++)
        {
            Vector2 dir = ApplySpread(baseDir, CurrentSpread);
            SpawnBullet(gunData, gunPoint, dir);
        }

        // 반동 증가
        float recoilAdd = GetFloat(gunData, 0f, "recoilAmount");
        CurrentSpread += recoilAdd;

        if (respectFireRate) cooldown = 1f / fireRatePerSec;

        if (gunData.muzzleFlashPrefab)
        {
            var fx = Instantiate(gunData.muzzleFlashPrefab, gunPoint.position, gunPoint.rotation);
            Destroy(fx, 0.05f);
        }

        // 간단한 사운드 예시 (표시명 기반)
        string shown = GetString(gunData, "weaponName") ?? GetString(gunData, "displayName");
        if (!string.IsNullOrEmpty(shown))
        {
            if (shown.Contains("Pistol"))
                WeaponSoundManager.Instance?.PlayPistolShootSound(transform.position);
            else if (shown.Contains("Rifle"))
                WeaponSoundManager.Instance?.PlayRifleShootSound(transform.position);
        }

        return true;
    }

    public bool Reload()
    {
        if (gunData == null) return false;
        if (currentMagazine >= capMagazine) return false;
        if (currentAmmo <= 0) return false;

        int need = capMagazine - currentMagazine;
        int move = Mathf.Min(need, currentAmmo);

        currentMagazine += move;
        currentAmmo -= move;

        cooldown = 0.5f;

        WeaponManager.Instance?.OnAmmoChanged?.Invoke();
        WeaponManager.Instance?.OnReloaded?.Invoke();
        WeaponSoundManager.Instance?.PlayReloadSound(transform.position);
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
            lifetime: projLife,
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

    // === 내부 전용: 탄약 쓰기 ===
    internal int AddAmmo(int amount)
    {
        if (gunData == null || amount <= 0) return 0;
        int before = currentAmmo;
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, capReserve);
        int gained = currentAmmo - before;
        if (gained != 0) WeaponManager.Instance?.OnAmmoChanged?.Invoke();
        return gained;
    }

    internal void ForceSetAmmo(int magazine, int reserve)
    {
        currentMagazine = Mathf.Clamp(magazine, 0, capMagazine);
        currentAmmo = Mathf.Clamp(reserve, 0, capReserve);
        WeaponManager.Instance?.OnAmmoChanged?.Invoke();
    }

    // ---- reflection helpers (신/구 스펙 동시 호환) ----
    private static string GetString(object obj, string field)
    {
        var f = obj.GetType().GetField(field, BindingFlags.Instance | BindingFlags.Public);
        return f != null ? f.GetValue(obj) as string : null;
    }
    private static int GetInt(object obj, int def, params string[] fields)
    {
        foreach (var nm in fields)
        {
            var f = obj.GetType().GetField(nm, BindingFlags.Instance | BindingFlags.Public);
            if (f != null)
            {
                var v = f.GetValue(obj);
                if (v is int iv) return iv;
            }
        }
        return def;
    }
    private static float GetFloat(object obj, float def, params string[] fields)
    {
        foreach (var nm in fields)
        {
            var f = obj.GetType().GetField(nm, BindingFlags.Instance | BindingFlags.Public);
            if (f != null)
            {
                var v = f.GetValue(obj);
                if (v is float fv) return fv;
                if (v is int iv) return iv; // 편의
            }
        }
        return def;
    }
    private static Vector2? GetVector2(object obj, params string[] fields)
    {
        foreach (var nm in fields)
        {
            var f = obj.GetType().GetField(nm, BindingFlags.Instance | BindingFlags.Public);
            if (f != null)
            {
                var v = f.GetValue(obj);
                if (v is Vector2 vec) return vec;
            }
        }
        return null;
    }
}
