using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverPopup : MonoBehaviour
{
    public TMP_Text weaponNameText;

    public TMP_Text scopeNameText;
    public TMP_Text flashlightNameText;
    public TMP_Text laserNameText;
    public TMP_Text gripNameText;
    public TMP_Text magazineNameText;
    public TMP_Text compensatorNameText;

    public TMP_Text categoryText;
    public TMP_Text damageText;
    public TMP_Text fireRateText;
    public TMP_Text ammoCapacityText;
    public TMP_Text accuracyText;
    public TMP_Text noiseText;
    public TMP_Text spreadText;
    public TMP_Text recoilControlText;
    public TMP_Text aimRangeText;
    public TMP_Text accuracyRecoveryText;
    public TMP_Text bulletSpeedText;
    public TMP_Text reloadSpeedText;
    public TMP_Text mobilityReductionText;

    public void Show(WeaponHoverData data)
    {
        weaponNameText.text = data.weaponName;

        scopeNameText.text = data.scopeName;
        flashlightNameText.text = data.flashlightName;
        laserNameText.text = data.laserName;
        gripNameText.text = data.gripName;
        magazineNameText.text = data.magazineName;
        compensatorNameText.text = data.compensatorName;

        categoryText.text = $"�з� : {data.category}";
        damageText.text = $"������ : {data.damage}";
        fireRateText.text = $"�߻�ӵ� : {data.fireRate}";
        ammoCapacityText.text = $"��ź�� : {data.ammoCapacity}";
        accuracyText.text = $"��Ȯ�� : {data.accuracy}";
        noiseText.text = $"���� : {data.noise}";
        spreadText.text = $"ź���� : {data.spread}";
        recoilControlText.text = $"�ݵ����� : {data.recoilControl}";
        aimRangeText.text = $"���ذŸ� : {data.aimRange}";
        accuracyRecoveryText.text = $"��Ȯ��ȸ�� : {data.accuracyRecovery}";
        bulletSpeedText.text = $"�Ѿ˼ӵ� : {data.bulletSpeed}";
        reloadSpeedText.text = $"�������ӵ� : {data.reloadSpeed}";
        mobilityReductionText.text = $"�̵����� : {data.mobilityReduction}";

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}