using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HoverPopup : MonoBehaviour
{
    public Color[] backgroundGradeColors; // 등급별 배경 색상 배열
    public Color[] borderGradeColors;     // 등급별 테두리 색상 배열
    [Space(20)]

    public TMP_Text weaponNameText;
    public Image backImage;

    [Space(20)]
    public TMP_Text categoryText;
    public TMP_Text damageText;
    public TMP_Text rpmText;
    public TMP_Text noiseText;
    public TMP_Text reloadSpeedText;
    public TMP_Text ammoCapacityText;

    [Space(20)]
    public TMP_Text accuracyText;
    public TMP_Text precisionText;
    public TMP_Text stabilityText;

    [Space(20)]
    public TMP_Text aimAccuracyText;
    public TMP_Text aimPrecisionText;
    public TMP_Text aimStabilityText;
    public TMP_Text aimDistanceText;

    public void Show(GunData gun)
    {
        if (gun == null) { gameObject.SetActive(false); return; }

        // 표시 이름: weaponName → displayName(구스펙) → ScriptableObject name
        weaponNameText.text = GetDisplayName(gun);

        // 분류: phaseTag / subType
        // var cat = ReadStr(gun, "phaseTag") ?? "Unknown";
        var cat = gun.prefabInfo.phaseTag;
        var sub = ReadStr(gun, "subType");
        categoryText.text = string.IsNullOrEmpty(sub) ? $"분류 : {cat}" : $"분류 : {cat} / {sub} / 총 종류";

        damageText.text = $"데미지 : {ReadStr(gun, "damage") ?? "-"}";
        rpmText.text = $"RPM : {ReadStr(gun, "rpm") ?? ReadStr(gun, "fireRate") ?? "-"}";
        // 소음: volume(신) → noise(구)
        noiseText.text = $"소음 : {ReadStr(gun, "volume") ?? ReadStr(gun, "noise") ?? "-"}";
        // 재장전속도: reloadSpeed(신, %) → reloadSpeedMul(구)
        reloadSpeedText.text = $"재장전 시간 : {ReadStr(gun, "reloadSpeed") ?? ReadStr(gun, "reloadSpeedMul") ?? "-"}";
        // 장탄수: maxAmmo(신) → maxMagazine(구) → magazine(아주 구)
        ammoCapacityText.text = $"장탄수 : {ReadStr(gun, "maxAmmo") ?? ReadStr(gun, "maxMagazine") ?? ReadStr(gun, "magazine") ?? "-"}";


        // 정확도: accuracy(신) → aimAccuracy → accuracyDeg(구)
        accuracyText.text = $"정확도 : {ReadStr(gun, "accuracy") ?? "-"}";
        // 정밀도: precision(신, 0~100) → spread(구) → accuracyDeg(구)
        precisionText.text = $"정밀도 : {ReadStr(gun, "precision") ?? "-"}";
        // 안정성 : stability
        stabilityText.text = $"안정성 : {ReadStr(gun, "stability") ?? "-"}";


        // 조준 시 정확도: aimAccuracy(신) → aimAccuracy → accuracyDeg(구)
        aimAccuracyText.text = $"조준 시 정확도 : {ReadStr(gun, "aimAccuracy") ?? "-"}";
        // 조준 시 정밀도 : aimPrecision(신, 0~100)
        aimPrecisionText.text = $"조준 시 정밀도 : {ReadStr(gun, "aimPrecision") ?? "-"}";
        // 조준 시 안정성 : aimStability
        aimStabilityText.text = $"조준 시 안정성 : {ReadStr(gun, "aimStability") ?? "-"}";

        // 조준 배율 : aimDistance
        string aimDistanceStr = ReadStr(gun, "aimDistance") ?? "-";
        float aimDistanceValue = float.Parse(aimDistanceStr) * 100f;

        aimDistanceText.text = $"조준 배율 : {aimDistanceValue}%";

        // 등급별 색상 적용
        int gradeIndex = Mathf.Clamp(gun.Grade, 0, backgroundGradeColors.Length - 1);
        backImage.color = borderGradeColors[gradeIndex];
        weaponNameText.color = backgroundGradeColors[gradeIndex];

        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    // --- helpers ---
    private static string GetDisplayName(GunData d)
    {
        var n = ReadStr(d, "weaponName") ?? ReadStr(d, "displayName");
        return string.IsNullOrEmpty(n) ? d.weaponName : n;
    }

    public static string ReadStr(object obj, string field)
    {
        var f = obj.GetType().GetField(field);
        if (f == null) return null;
        var v = f.GetValue(obj);
        return v != null ? v.ToString() : null;
    }
}
