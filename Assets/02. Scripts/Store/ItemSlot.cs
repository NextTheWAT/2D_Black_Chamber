using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler    // 마우스커서가 UI위로 왔을때 호출, UI에서 벗어났을때 호출
{
    public HoverPopup hoverPopup;
    public WeaponHoverData weaponHoverData;

    private bool RightSideSlot;
    public Vector2 offset = new Vector2(400f, 0f);

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
            StartCoroutine(ShowPopupDelay());
        }
    }

    private IEnumerator ShowPopupDelay()    // 클릭씹히는거 막기
    {
        yield return null;
        Vector3 newPos = transform.position;
        newPos.x += RightSideSlot ? -offset.x : offset.x;
        newPos.y += offset.y;

        hoverPopup.transform.position = newPos;
        hoverPopup.Show(weaponHoverData);
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