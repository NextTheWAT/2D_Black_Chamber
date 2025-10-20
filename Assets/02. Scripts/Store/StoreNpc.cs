using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreNpc : MonoBehaviour, Iinteraction
{
    public GameObject weaponsPanel;
    public GameObject toolsPanel;

    private bool isOpen = false;

    public void Interaction(Transform interactor)
    {
        if (!isOpen)
            OpenStore();
        else
            CloseStore(); 
    }

    public void OpenStore()
    {
        weaponsPanel.SetActive(true);
        toolsPanel.SetActive(false);
        Time.timeScale = 0f;
        isOpen = true;
    }

    public void CloseStore()
    {
        weaponsPanel.SetActive(false);
        toolsPanel.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }
}

