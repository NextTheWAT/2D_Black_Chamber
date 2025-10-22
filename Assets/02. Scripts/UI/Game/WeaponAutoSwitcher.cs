using UnityEngine;
using System.Collections;
using Constants; // GamePhase

public class WeaponAutoSwitcher : MonoBehaviour
{
    [Header("Optional Knife Fallback")]
    [SerializeField] private int knifeIndex = -1;      // 탄약 0일 때 전환(없으면 -1)
    [SerializeField] private float ammoCheckInterval = 0.1f;

    private WeaponManager WM => WeaponManager.Instance;
    private GameManager GM => GameManager.Instance;

    private void OnEnable()
    {
        if (GM) GM.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDisable()
    {
        if (GM) GM.OnPhaseChanged -= OnPhaseChanged;
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
            yield return new WaitForSeconds(ammoCheckInterval);
        }
    }

    private void OnPhaseChanged(GamePhase phase) => UpdateByContext();

    private void UpdateByContext()
    {
        if (WM == null) return;

        var phase = GM ? GM.CurrentPhase : GamePhase.Stealth;

        // 현재 페이즈의 목표 무기 얻기 (Manager의 퍼블릭 헬퍼 사용)
        Shooter target =
            (phase == GamePhase.Combat) ? WM.GetCombatShooter()
                                        : WM.GetStealthShooter();

        int targetAmmoTotal = (target != null)
            ? (target.CurrentMagazine + target.CurrentAmmo)
            : 0;

        // 탄이 없으면: 칼 인덱스가 유효할 때만 강제 전환
        if (targetAmmoTotal <= 0)
        {
            if (knifeIndex >= 0) WM.EquipByIndex(knifeIndex);
            return;
        }

        // 탄이 있고, 현재 무기가 목표 무기가 아니면 전환
        if (target != null && WM.CurrentWeapon != target)
        {
            // 슬롯 인덱스를 모를 수 있으니 Data 기준으로 전환 (Manager API)
            if (target.gunData != null)
                WM.EquipByData(target.gunData);
            else
                // 데이터가 없을 예외 상황 대비: 현재 페이즈에 맞춰 재적용
                WM.ApplyPhaseWeapon(phase);
        }
    }
}
