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
        // ���� ��ư ���� �� ����â �߰�
        purchasePanel.SetActive(true);
    }

    public void EnoughMoneyPopup(bool hasEnoughMoney)
    {
        if (hasEnoughMoney)
            completePanel.SetActive(true);
        else
            shortagePanel.SetActive(true);
    }

    public void ShowCompletePopup()
    {
        // �ܾ� ����Ҷ� ���Ž� ���ſϷ�â �߰�
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
