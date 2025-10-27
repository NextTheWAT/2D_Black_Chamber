using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoryNpc : MonoBehaviour, Iinteraction
{
    public GameObject arMoryPanel;

    private bool isOpen = false;

    public void Interaction(Transform interactor)
    {
        if (!isOpen)
            Open();
        else
            Close(); 
    }

    public void Open()
    {
        arMoryPanel.SetActive(true);
        Time.timeScale = 0f;
        isOpen = true;
    }

    public void Close()
    {
        arMoryPanel.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }
}

