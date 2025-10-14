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

        categoryText.text = $"분류 : {data.category}";
        damageText.text = $"데미지 : {data.damage}";
        fireRateText.text = $"발사속도 : {data.fireRate}";
        ammoCapacityText.text = $"장탄수 : {data.ammoCapacity}";
        accuracyText.text = $"정확도 : {data.accuracy}";
        noiseText.text = $"소음 : {data.noise}";
        spreadText.text = $"탄퍼짐 : {data.spread}";
        recoilControlText.text = $"반동제어 : {data.recoilControl}";
        aimRangeText.text = $"조준거리 : {data.aimRange}";
        accuracyRecoveryText.text = $"정확도회복 : {data.accuracyRecovery}";
        bulletSpeedText.text = $"총알속도 : {data.bulletSpeed}";
        reloadSpeedText.text = $"재장전속도 : {data.reloadSpeed}";
        mobilityReductionText.text = $"이동감소 : {data.mobilityReduction}";

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}