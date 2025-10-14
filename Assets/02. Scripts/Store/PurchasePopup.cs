using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasePopup : MonoBehaviour
{
    public GameObject purchasePanel;
    public GameObject shortagePanel;
    public GameObject completePanel;

    private void Start()
    {
        CloseShortagePopup();
        CloseCompletePopup();
        ClosePurchasePopup();

    }

    public void ShowPurchasePopup()
    {
        // 슬롯 버튼 누를 시 구매창 뜨게
        purchasePanel.SetActive(true);
    }

    public void EnoughMoneyPopup(bool hasEnoughMoney)   // 가진금액에따라 다른 팝업뜨게 (추후 수정)
    {
        if (hasEnoughMoney)
            completePanel.SetActive(true);         // 잔액 충분할때 구매시 구매완료창 뜨게
        else
            shortagePanel.SetActive(true);
    }

    public void ShowCompletePopup()
    {
        completePanel.SetActive(true);
    }

    public void CloseShortagePopup()
    {
        shortagePanel.SetActive(false);
    }

    public void CloseCompletePopup()
    {
        completePanel.SetActive(false);
    }

    public void ClosePurchasePopup()
    {
        purchasePanel.SetActive(false);
    }
}
