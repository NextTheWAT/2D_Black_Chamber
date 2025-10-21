using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseCondition : MonoBehaviour
{
    public GameObject shortagePanel;
    public GameObject completePanel;

    public void EnoughMoneyPopup(bool hasEnoughMoney)   // 가진금액에따라 다른 팝업뜨게 (추후 수정)
    {
        if (hasEnoughMoney)
            ShowCompletePopup();        // 잔액 충분할때 구매시 구매완료창 뜨게
        else
            ShowShortagePopup();
    }

    public void ShowCompletePopup()
    {
        completePanel.SetActive(true);
    }

    public void ShowShortagePopup()
    {
        shortagePanel.SetActive(true);
    }

    public void CloseShortagePopup()
    {
        shortagePanel.SetActive(false);
    }

    public void CloseCompletePopup()
    {
        completePanel.SetActive(false);
    }
}
