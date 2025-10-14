using UnityEngine;
using System.Collections;

public class WeaponAutoSwitcher : MonoBehaviour
{
    [Header("Optional Knife Fallback")]
    [SerializeField] private int knifeIndex = -1; // 탄약 0일 때 전환 (없으면 -1)

    [SerializeField] private float ammoCheckInterval = 0.1f; // 반응성 향상

    private WeaponManager WM => WeaponManager.Instance;
    private GameManager GM => GameManager.Instance;

    public int KnifeIndex { get => knifeIndex; set => knifeIndex = value; }
    public float AmmoCheckInterval { get => ammoCheckInterval; set => ammoCheckInterval = value; }

    private void OnEnable()
    {
        if (GM != null) GM.OnPhaseChanged += OnPhaseChanged;
        // AmmoChanged 리스너는 루프 위험이 있어 제거. 폴링 + 페이즈 변경만 사용.
    }

    private void OnDisable()
    {
        if (GM != null) GM.OnPhaseChanged -= OnPhaseChanged;
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

    private void OnPhaseChanged(Constants.GamePhase phase) => UpdateByContext();

    private void UpdateByContext()
    {
        if (WM == null || WM.weaponSlots == null || WM.weaponSlots.Length == 0) return;

        var phase = GM.CurrentPhase;
        // 현재 페이즈의 무기(스텔스=권총, 난전=주무기)를 기준으로 탄약 판단
        var targetSlot = (phase == Constants.GamePhase.Combat) ? WM.CombatSlotIndex : WM.StealthSlotIndex;

        int targetAmmoTotal = 0;
        if (targetSlot >= 0 && targetSlot < WM.weaponSlots.Length && WM.weaponSlots[targetSlot] != null)
        {
            var s = WM.weaponSlots[targetSlot];
            targetAmmoTotal = s.CurrentMagazine + s.CurrentAmmo;
        }

        if (targetAmmoTotal <= 0)
        {
            // 해당 페이즈 무기에 탄이 없으면 칼로 전환 (설정된 경우)
            if (knifeIndex >= 0 && knifeIndex < WM.weaponSlots.Length && WM.CurrentWeaponIndex != knifeIndex)
                WM.CurrentWeaponIndex = knifeIndex;
            return;
        }

        // 탄이 생겼으면 (예: 칼 들고 있을 때 픽업) 해당 페이즈 무기로 복귀
        if (targetSlot >= 0 && WM.CurrentWeaponIndex != targetSlot)
            WM.CurrentWeaponIndex = targetSlot;
    }
}
