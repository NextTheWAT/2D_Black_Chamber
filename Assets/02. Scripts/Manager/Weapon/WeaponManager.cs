using Constants;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WeaponManager : Singleton<WeaponManager>
{
    [Serializable] public class WeaponChangedEvent : UnityEvent<Shooter> { }
    public WeaponChangedEvent OnWeaponChanged = new();         // HUD/�ִ�/���尡 ����

    public UnityEvent OnAmmoChanged { get; private set; } = new UnityEvent();
    public UnityEvent OnReloaded { get; private set; } = new UnityEvent();

    public GunData[] intializeDatas;
    [HideInInspector] public Shooter[] weaponSlots;

    [SerializeField] private int initializeIndex = 0; // �ʱ� ���� �ε���
    private int currentIndex = 0;

    private int sharedMagazine = 6;
    private int sharedReserve = 0;

    public Shooter CurrentWeapon
        => (weaponSlots != null && weaponSlots.Length > 0) ? weaponSlots[currentIndex] : null;

    public int CurrentWeaponIndex
    {
        get => currentIndex;
        set
        {
            if (weaponSlots == null || weaponSlots.Length == 0) return;

            // 1) ��ȯ ����: ���� ������ ź �� ���
            var oldWeapon = CurrentWeapon;
            if (oldWeapon != null)
            {
                sharedMagazine = oldWeapon.CurrentMagazine;
                sharedReserve = oldWeapon.CurrentAmmo;
            }

            // 2) �ε��� ����
            currentIndex = value % weaponSlots.Length;

            // 3) ��ȯ ����: �� ���⿡ ���� ź �� ����
            var newWeapon = CurrentWeapon;
            if (newWeapon != null)
            {
                newWeapon.SetAmmo(sharedMagazine, sharedReserve);
            }

            // 4) �̺�Ʈ �˸�
            OnWeaponChanged.Invoke(newWeapon);
            OnAmmoChanged.Invoke();
        }
    }

    private void OnEnable()
    {
        if (AppIsQuitting) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDisable()
    {
        if (AppIsQuitting) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(GamePhase phase)
    {
        if (phase == Constants.GamePhase.Combat)
            CurrentWeaponIndex = 1;
        else
            CurrentWeaponIndex = 0;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �� ��ȯ �� ���� �ʱ�ȭ
        InitializeWeapon();
    }

    public void InitializeWeapon()
    {
        if (weaponSlots != null)
        {
            foreach (var weapon in weaponSlots)
            {
                if (weapon != null)
                    Destroy(weapon.gameObject);
            }
        }

        weaponSlots = new Shooter[intializeDatas.Length];

        for (int i = 0; i < intializeDatas.Length; i++)
        {
            var go = new GameObject(intializeDatas[i].displayName);
            go.transform.SetParent(GameManager.Instance.Player);
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            var shooter = go.AddComponent<Shooter>();
            shooter.Initialize(intializeDatas[i]); // Shooter�� 6/0���� �ʱ�ȭ
            weaponSlots[i] = shooter;
        }

        sharedMagazine = 6;
        sharedReserve = 0;

        CurrentWeaponIndex = initializeIndex;
        OnWeaponChanged.Invoke(CurrentWeapon);
        OnAmmoChanged.Invoke();
    }

    public void SetShooterLocked(bool locked)
    {
        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot != null)
                weaponSlot.shooterLocked = locked;
        }
    }

    public int GetMagazine() => CurrentWeapon ? CurrentWeapon.CurrentMagazine : 0;
    public int GetReserve() => CurrentWeapon ? CurrentWeapon.CurrentAmmo : 0;
    public bool RequestReload() => CurrentWeapon.Reload();
}
