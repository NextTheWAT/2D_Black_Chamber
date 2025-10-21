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

    public void Show(GunData gun)
    {
        if (!gun) { gameObject.SetActive(false); return; }

        weaponNameText.text = gun.displayName;

        // 부착물 이름(없으면 "-")
        scopeNameText.text = ReadStr(gun, "scopeName") ?? "-";
        flashlightNameText.text = ReadStr(gun, "flashlightName") ?? "-";
        laserNameText.text = ReadStr(gun, "laserName") ?? "-";
        gripNameText.text = ReadStr(gun, "gripName") ?? "-";
        magazineNameText.text = ReadStr(gun, "magazineName") ?? "-";
        compensatorNameText.text = ReadStr(gun, "compensatorName") ?? "-";

        // 분류: phaseTag 우선, 없으면 "Unknown"
        var cat = ReadStr(gun, "phaseTag") ?? "Unknown";
        var sub = ReadStr(gun, "subType");
        categoryText.text = string.IsNullOrEmpty(sub) ? $"분류 : {cat}" : $"분류 : {cat} / {sub}";

        damageText.text = $"데미지 : {ReadStr(gun, "damage") ?? "-"}";
        fireRateText.text = $"발사속도 : {ReadStr(gun, "rpm") ?? ReadStr(gun, "fireRate") ?? "-"}";
        ammoCapacityText.text = $"장탄수 : {ReadStr(gun, "maxMagazine") ?? ReadStr(gun, "magazine") ?? "-"}";
        accuracyText.text = $"정확도 : {ReadStr(gun, "accuracyDeg") ?? ReadStr(gun, "accuracy") ?? "-"}";
        noiseText.text = $"소음 : {ReadStr(gun, "noise") ?? "-"}";
        spreadText.text = $"탄퍼짐 : {ReadStr(gun, "spread") ?? "-"}";
        recoilControlText.text = $"반동제어 : {ReadStr(gun, "recoilControl") ?? ReadStr(gun, "recoilRecovery") ?? "-"}";
        aimRangeText.text = $"조준거리 : {ReadStr(gun, "aimRange") ?? ReadStr(gun, "aimDistancePct") ?? "-"}";
        accuracyRecoveryText.text = $"정확도회복 : {ReadStr(gun, "accuracyRecovery") ?? "-"}";
        bulletSpeedText.text = $"총알속도 : {ReadStr(gun, "bulletSpeed") ?? "-"}";
        reloadSpeedText.text = $"재장전속도 : {ReadStr(gun, "reloadSpeedMul") ?? ReadStr(gun, "reloadSpeed") ?? "-"}";
        mobilityReductionText.text = $"이동감소 : {ReadStr(gun, "movePenaltyPct") ?? ReadStr(gun, "mobilityReduction") ?? "-"}";

        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    private static string ReadStr(object obj, string field)
    {
        var f = obj.GetType().GetField(field);
        if (f == null) return null;
        var v = f.GetValue(obj);
        return v != null ? v.ToString() : null;
    }
}
