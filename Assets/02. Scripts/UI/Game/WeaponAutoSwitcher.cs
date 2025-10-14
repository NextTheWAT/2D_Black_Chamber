using UnityEngine;
using System.Collections;

public class WeaponAutoSwitcher : MonoBehaviour
{
    [Header("Optional Knife Fallback")]
    [SerializeField] private int knifeIndex = -1; // ź�� 0�� �� ��ȯ (������ -1)

    [SerializeField] private float ammoCheckInterval = 0.1f; // ������ ���

    private WeaponManager WM => WeaponManager.Instance;
    private GameManager GM => GameManager.Instance;

    public int KnifeIndex { get => knifeIndex; set => knifeIndex = value; }
    public float AmmoCheckInterval { get => ammoCheckInterval; set => ammoCheckInterval = value; }

    private void OnEnable()
    {
        if (GM != null) GM.OnPhaseChanged += OnPhaseChanged;
        // AmmoChanged �����ʴ� ���� ������ �־� ����. ���� + ������ ���游 ���.
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
        // ���� �������� ����(���ڽ�=����, ����=�ֹ���)�� �������� ź�� �Ǵ�
        var targetSlot = (phase == Constants.GamePhase.Combat) ? WM.CombatSlotIndex : WM.StealthSlotIndex;

        int targetAmmoTotal = 0;
        if (targetSlot >= 0 && targetSlot < WM.weaponSlots.Length && WM.weaponSlots[targetSlot] != null)
        {
            var s = WM.weaponSlots[targetSlot];
            targetAmmoTotal = s.CurrentMagazine + s.CurrentAmmo;
        }

        if (targetAmmoTotal <= 0)
        {
            // �ش� ������ ���⿡ ź�� ������ Į�� ��ȯ (������ ���)
            if (knifeIndex >= 0 && knifeIndex < WM.weaponSlots.Length && WM.CurrentWeaponIndex != knifeIndex)
                WM.CurrentWeaponIndex = knifeIndex;
            return;
        }

        // ź�� �������� (��: Į ��� ���� �� �Ⱦ�) �ش� ������ ����� ����
        if (targetSlot >= 0 && WM.CurrentWeaponIndex != targetSlot)
            WM.CurrentWeaponIndex = targetSlot;
    }
}
