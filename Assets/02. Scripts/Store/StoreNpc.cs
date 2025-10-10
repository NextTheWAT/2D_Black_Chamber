using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreNpc : MonoBehaviour, Iinteraction
{
    public GameObject targetCanvas;

    private bool isOpen = false;

    public void Interaction()
    {
        if (targetCanvas == null)
            return;

    isOpen = !isOpen;
    targetCanvas.SetActive(isOpen);
    }
}
