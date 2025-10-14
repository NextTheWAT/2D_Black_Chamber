using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler    // ���콺Ŀ���� UI���� ������ ȣ��, UI���� ������� ȣ��
{
    public HoverPopup hoverPopup;
    public WeaponHoverData weaponHoverData;

    private bool isHovering = false;    // ���� ���� ����

    private void Start()
    {
        if (hoverPopup != null)
            hoverPopup.Hide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering && hoverPopup != null)
        {
            isHovering = true;
            hoverPopup.Show(weaponHoverData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHovering && hoverPopup != null)
        {
            isHovering = false;
            hoverPopup.Hide();
        }
    }
}