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

        // 표시 이름: weaponName → displayName(구스펙) → ScriptableObject name
        weaponNameText.text = GetDisplayName(gun);

        // 부착물 이름(없으면 "-") — 프로젝트에 실제 필드가 없다면 자동으로 "-"
        scopeNameText.text = ReadStr(gun, "scopeName") ?? "-";
        flashlightNameText.text = ReadStr(gun, "flashlightName") ?? "-";
        laserNameText.text = ReadStr(gun, "laserName") ?? "-";
        gripNameText.text = ReadStr(gun, "gripName") ?? "-";
        magazineNameText.text = ReadStr(gun, "magazineName") ?? "-";
        compensatorNameText.text = ReadStr(gun, "compensatorName") ?? "-";

        // 분류: phaseTag / subType
        var cat = ReadStr(gun, "phaseTag") ?? "Unknown";
        var sub = ReadStr(gun, "subType");
        categoryText.text = string.IsNullOrEmpty(sub) ? $"분류 : {cat}" : $"분류 : {cat} / {sub}";

        damageText.text = $"데미지 : {ReadStr(gun, "damage") ?? "-"}";
        fireRateText.text = $"발사속도 : {ReadStr(gun, "rpm") ?? ReadStr(gun, "fireRate") ?? "-"}";

        // 장탄수: maxAmmo(신) → maxMagazine(구) → magazine(아주 구)
        ammoCapacityText.text = $"장탄수 : {ReadStr(gun, "maxAmmo") ?? ReadStr(gun, "maxMagazine") ?? ReadStr(gun, "magazine") ?? "-"}";

        // 정확도: accuracy(신) → aimAccuracy → accuracyDeg(구)
        accuracyText.text = $"정확도 : {ReadStr(gun, "accuracy") ?? ReadStr(gun, "aimAccuracy") ?? ReadStr(gun, "accuracyDeg") ?? "-"}";

        // 소음: volume(신) → noise(구)
        noiseText.text = $"소음 : {ReadStr(gun, "volume") ?? ReadStr(gun, "noise") ?? "-"}";

        // 탄퍼짐: precision(신, 0~100) → spread(구) → accuracyDeg(구)
        spreadText.text = $"탄퍼짐 : {ReadStr(gun, "precision") ?? ReadStr(gun, "spread") ?? ReadStr(gun, "accuracyDeg") ?? "-"}";

        // 반동제어/회복: recoilRecovery(신) → recoilControl(구)
        recoilControlText.text = $"반동제어 : {ReadStr(gun, "recoilRecovery") ?? ReadStr(gun, "recoilControl") ?? "-"}";

        // 조준거리: aimDistance(신, %) → aimRange(구) → aimDistancePct(구)
        aimRangeText.text = $"조준거리 : {ReadStr(gun, "aimDistance") ?? ReadStr(gun, "aimRange") ?? ReadStr(gun, "aimDistancePct") ?? "-"}";

        // 정확도 회복(프로젝트에 없으면 '-')
        accuracyRecoveryText.text = $"정확도회복 : {ReadStr(gun, "accuracyRecovery") ?? "-"}";

        bulletSpeedText.text = $"총알속도 : {ReadStr(gun, "bulletSpeed") ?? "-"}";

        // 재장전속도: reloadSpeed(신, %) → reloadSpeedMul(구)
        reloadSpeedText.text = $"재장전속도 : {ReadStr(gun, "reloadSpeed") ?? ReadStr(gun, "reloadSpeedMul") ?? "-"}";

        // 이동속도 배율/감소: moveSpeedModifier(신, %) → movePenaltyPct(구) → mobilityReduction(구)
        mobilityReductionText.text = $"이동감소 : {ReadStr(gun, "moveSpeedModifier") ?? ReadStr(gun, "movePenaltyPct") ?? ReadStr(gun, "mobilityReduction") ?? "-"}";

        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    // --- helpers ---
    private static string GetDisplayName(GunData d)
    {
        var n = ReadStr(d, "weaponName") ?? ReadStr(d, "displayName");
        return string.IsNullOrEmpty(n) ? d.name : n;
    }

    private static string ReadStr(object obj, string field)
    {
        var f = obj.GetType().GetField(field);
        if (f == null) return null;
        var v = f.GetValue(obj);
        return v != null ? v.ToString() : null;
    }
}
