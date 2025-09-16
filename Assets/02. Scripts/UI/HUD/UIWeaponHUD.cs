using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWeaponHUD : MonoBehaviour //���� ������, ź�� ǥ��
{
    [Header("UI Refs")]
    [SerializeField] private Image iconImage; //���� ������
    [SerializeField] private TMP_Text ammoText; //ź�� ǥ��

    public void SetAmmo(int magazine, int reserve) //���� ������ ź�� �� ����
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
