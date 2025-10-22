using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoryNpc : MonoBehaviour, Iinteraction
{
    public GameObject weaponsPanel;

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
        weaponsPanel.SetActive(true);
        Time.timeScale = 0f;
        isOpen = true;
    }

    public void Close()
    {
        weaponsPanel.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }
}

