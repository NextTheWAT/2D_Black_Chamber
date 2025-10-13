using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreTabBtn : MonoBehaviour
{
    public GameObject weaponsCanvas;
    public GameObject toolsCanvas;
    public GameObject weaponsButtonBar;
    public GameObject toolsButtonBar;

    private void Start()
    {
        ShowWeapons();
    }

    public void ShowWeapons()
    {
        weaponsCanvas.SetActive(true);
        toolsCanvas.SetActive(false);
        weaponsButtonBar.SetActive(true);
        toolsButtonBar.SetActive(false);
    }

    public void ShowTools()
    {
        weaponsCanvas.SetActive(false);
        toolsCanvas.SetActive(true);
        weaponsButtonBar.SetActive(false);
        toolsButtonBar.SetActive(true);
    }
}
