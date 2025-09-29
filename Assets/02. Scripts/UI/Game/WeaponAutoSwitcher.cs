using UnityEngine;
using System.Collections;

public class WeaponAutoSwitcher : MonoBehaviour
{
    [Header("Weapon Slots Index (WeaponManager.weaponSlots 순서)")]
    [SerializeField] private int pistolIndex = 0; // 잠입
    [SerializeField] private int rifleIndex = 1; // 난전
    [SerializeField] private int knifeIndex = 2; // 총알 없음

    [SerializeField] private float ammoCheckInterval = 0.25f;

    private WeaponManager WM => WeaponManager.Instance;
    private GameManager GM => GameManager.Instance;

    public int PistolIndex { get => pistolIndex; set => pistolIndex = value; }
    public int RifleIndex { get => rifleIndex; set => rifleIndex = value; }
    public int KnifeIndex { get => knifeIndex; set => knifeIndex = value; }
    public float AmmoCheckInterval { get => ammoCheckInterval; set => ammoCheckInterval = value; }

    private void OnEnable()
    {
        if (GM != null) GM.OnPhaseChanged += OnPhaseChanged;
        if (WM != null) WM.OnAmmoChanged.AddListener(UpdateByContext);
    }

    private void OnDisable()
    {
        if (GM != null) GM.OnPhaseChanged -= OnPhaseChanged;
        if (WM != null) WM.OnAmmoChanged.RemoveListener(UpdateByContext);
    }

    private void Start()
    {
        UpdateByContext();
        StartCoroutine(CoPollAmmo());
    }

    private IEnumerator CoPollAmmo()
    {
        while (true)
        {
            UpdateByContext();
            yield return new WaitForSeconds(AmmoCheckInterval);
        }
    }

    private void OnPhaseChanged(Constants.GamePhase _) => UpdateByContext();

    private void UpdateByContext()
    {
        if (WM == null || WM.weaponSlots == null || WM.weaponSlots.Length == 0) return;

        int totalAmmo = SafeGetMagazine() + SafeGetReserve();

        // 1) 탄약 0 > 칼
        if (totalAmmo <= 0)
        {
            SetWeaponIfDifferent(KnifeIndex);
            return;
        }

        // 2) 탄약 있음 > 상태에 따라 권총/라이플
        bool combat = (GM != null && GM.IsCombat);
        // SetWeaponIfDifferent(combat ? RifleIndex : PistolIndex);
    }

    private int SafeGetMagazine() { try { return WM.GetMagazine(); } catch { return 0; } }
    private int SafeGetReserve() { try { return WM.GetReserve(); } catch { return 0; } }

    private void SetWeaponIfDifferent(int slotIndex)
    {
        if (slotIndex == knifeIndex) return;
        slotIndex = Mathf.Clamp(slotIndex, 0, WM.weaponSlots.Length - 1);
        if (WM.CurrentWeaponIndex == slotIndex) return;
        WM.CurrentWeaponIndex = slotIndex;
    }
}
