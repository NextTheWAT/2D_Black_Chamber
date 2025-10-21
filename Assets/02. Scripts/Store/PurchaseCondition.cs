using UnityEngine;

public class PurchaseCondition : MonoBehaviour
{
    public GameObject shortagePanel;   // 잔액 부족
    public GameObject completePanel;   // 구매 완료

    /// <summary>
    /// 구매 성공/실패에 따라 팝업 분기.
    /// (true = 구매 성공 → 완료창, false = 실패 → 부족창)
    /// </summary>
    public void EnoughMoneyPopup(bool purchased)
    {
        HideAll();
        if (purchased) ShowCompletePopup();
        else ShowShortagePopup();
    }

    public void ShowCompletePopup()
    {
        HideAll();
        if (completePanel) completePanel.SetActive(true);
    }

    public void ShowShortagePopup()
    {
        HideAll();
        if (shortagePanel) shortagePanel.SetActive(true);
    }

    public void CloseShortagePopup()
    {
        if (shortagePanel) shortagePanel.SetActive(false);
    }

    public void CloseCompletePopup()
    {
        if (completePanel) completePanel.SetActive(false);
    }

    private void HideAll()
    {
        if (shortagePanel) shortagePanel.SetActive(false);
        if (completePanel) completePanel.SetActive(false);
    }
}
