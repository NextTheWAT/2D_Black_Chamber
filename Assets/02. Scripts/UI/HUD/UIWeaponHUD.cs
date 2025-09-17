using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWeaponHUD : MonoBehaviour //무기 아이콘, 탄약 표시
{
    [Header("UI Refs")]
    [SerializeField] private Image iconImage; //무기 아이콘
    [SerializeField] private TMP_Text ammoText; //탄약 표시

    public void SetAmmo(int magazine, int reserve) //현재 소지한 탄약 수 갱신
    {
        if (ammoText != null)
            ammoText.text = $"{magazine} / {reserve}";
    }

    public void SetIcon(Sprite sprite)
    {
        if (iconImage != null)
            iconImage.sprite = sprite;
    }
}
