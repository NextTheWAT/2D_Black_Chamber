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

    public void EnoughMoneyPopup(bool hasEnoughMoney)   // �����ݾ׿����� �ٸ� �˾��߰� (���� ����)
    {
        if (hasEnoughMoney)
            completePanel.SetActive(true);         // �ܾ� ����Ҷ� ���Ž� ���ſϷ�â �߰�
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
