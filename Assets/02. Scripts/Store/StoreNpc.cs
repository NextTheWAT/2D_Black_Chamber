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

        if (!isOpen)
            OpenStore();
        else
            CloseStore(); 
    }

    public void OpenStore()
    {
        targetCanvas.SetActive(true);
        Time.timeScale = 0f;
        isOpen = true;
    }

    public void CloseStore()
    {
        if (targetCanvas == null)
            return;
        targetCanvas.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }
}

