using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler    // 마우스커서가 UI위로 왔을때 호출, UI에서 벗어났을때 호출
{
    public HoverPopup hoverPopup;
    public WeaponHoverData weaponHoverData;

    private bool isHovering = false;    // 무한 생성 방지

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