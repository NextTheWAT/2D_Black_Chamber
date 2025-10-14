using UnityEngine;

public partial class PlayerInputController
{
    [Header("UI Prompt (Reload)")]
    [SerializeField] private GameObject rKeyPrompt;   // RŰ �ִ� ������Ʈ(�⺻ ��Ȱ��)

    [Tooltip("����ź�� ���� ���� ǥ�� (true ����)")]
    [SerializeField] private bool showOnlyWhenReloadPossible = true;

    // --- �̺�Ʈ �ڵ鷯 ĳ�� (����/������) ---
    private void HandleWeaponChanged(Shooter _)
    {
        RefreshReloadPrompt();
    }
    private void HandleAmmoChanged()
    {
        RefreshReloadPrompt();
    }
    private void HandleReloaded()
    {
        RefreshReloadPrompt();
    }

    private void OnEnable()
    {
        var wm = WeaponManager.Instance;
        if (wm != null)
        {
            wm.OnWeaponChanged.AddListener(HandleWeaponChanged);
            wm.OnAmmoChanged.AddListener(HandleAmmoChanged);
            wm.OnReloaded.AddListener(HandleReloaded);
        }
        RefreshReloadPrompt();
    }

    private void OnDisable()
    {
        var wm = WeaponManager.Instance;
        if (wm != null)
        {
            wm.OnWeaponChanged.RemoveListener(HandleWeaponChanged);
            wm.OnAmmoChanged.RemoveListener(HandleAmmoChanged);
            wm.OnReloaded.RemoveListener(HandleReloaded);
        }
        if (rKeyPrompt != null) rKeyPrompt.SetActive(false);
    }

    private void Start()
    {
        // �� ���� �� �� �� �ݿ�
        RefreshReloadPrompt();
    }

    private void RefreshReloadPrompt()
    {
        if (rKeyPrompt == null)
            return;

        var wm = WeaponManager.Instance;
        if (wm == null || wm.CurrentWeapon == null)
        {
            rKeyPrompt.SetActive(false);
            return;
        }

        int mag = wm.GetMagazine(); // ���� źâ
        int reserve = wm.GetReserve(); // ����ź
        bool emptyMag = mag <= 0;

        bool show = emptyMag;
        if (showOnlyWhenReloadPossible)
            show &= (reserve > 0);   // ����ź ������ ����

        rKeyPrompt.SetActive(show);
    }
}
