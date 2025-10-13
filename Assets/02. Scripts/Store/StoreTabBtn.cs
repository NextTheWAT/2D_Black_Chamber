using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreTabBtn : MonoBehaviour
{
    public GameObject weaponsPanel;
    public GameObject toolsPanel;

    public GameObject weaponsButtonBar;
    public GameObject toolsButtonBar;

    private void Start()
    {
        weaponsPanel.SetActive(true);
        toolsPanel.SetActive(false);

        weaponsButtonBar.SetActive(true);
        toolsButtonBar.SetActive(false);
    }

    public void ShowWeapons()
    {
        if (weaponsPanel.activeSelf)
            return;

        weaponsPanel.SetActive(true);
        toolsPanel.SetActive(false);

        weaponsButtonBar.SetActive(true);
        toolsButtonBar.SetActive(false);
    }

    public void ShowTools()
    {
        if (toolsPanel.activeSelf)
            return;

        weaponsPanel.SetActive(false);
        toolsPanel.SetActive(true);

        weaponsButtonBar.SetActive(false);
        toolsButtonBar.SetActive(true);
    }
}
