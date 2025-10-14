using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverPopup;

    private void Start()
    {
        if (hoverPopup != null)
            hoverPopup.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverPopup != null)
            hoverPopup.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverPopup != null)
            hoverPopup.SetActive(false);
    }
}
