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
        // 슬롯 버튼 누를 시 구매창 뜨게 -> 짧은딜레이를 줘서 오히려 버튼 씹힘을 없애기
        StartCoroutine(ShowPurchasePopupDelayed());
    }

    private IEnumerator ShowPurchasePopupDelayed()
    {
        yield return null;
        purchasePanel.SetActive(true);
    }

    public void EnoughMoneyPopup(bool hasEnoughMoney)   // 가진금액에따라 다른 팝업뜨게 (추후 수정)
    {
        StartCoroutine(ShowEnoughMoneyPopup(hasEnoughMoney));
    }

    private IEnumerator ShowEnoughMoneyPopup(bool hasEnoughMoney)
    {
        yield return null;
        if (hasEnoughMoney)
            completePanel.SetActive(true);         // 잔액 충분할때 구매시 구매완료창 뜨게
        else
            shortagePanel.SetActive(true);
    }

    public void ShowCompletePopup()
    {
        StartCoroutine(ShowCompletePopupDelayed());
    }

    private IEnumerator ShowCompletePopupDelayed()
    {
        yield return null;
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
