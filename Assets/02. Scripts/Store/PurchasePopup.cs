using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasePopup : MonoBehaviour
{
    public GameObject purchasePanel;

    private void Start()
    {

    }

    public void ShowPurchasePopup()
    {
        purchasePanel.SetActive(true);
    }

    public void ClosePurchasePopup()
    {
        purchasePanel.SetActive(false);
    }
}