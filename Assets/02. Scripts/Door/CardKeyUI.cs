using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardKeyUI : MonoBehaviour
{
    public static CardKeyUI Instance;
    public TextMeshProUGUI cardKeyText;
    private int cardKeyCount = 0;

    public TextMeshProUGUI needCardKeyText;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UpdateCardKeyText();
    }
    public void AddCardKey()
    {
        cardKeyCount++;
        UpdateCardKeyText();
    }

    public void UseCardKey()
    {
        if (cardKeyCount > 0)
        {
            cardKeyCount--;
            UpdateCardKeyText();
        }
    }

    public void NeedCardKeyTxt()
    {
        needCardKeyText.text = "카드키 필요!";
        needCardKeyText.gameObject.SetActive(true);

        CancelInvoke(nameof(HideNeedCardKeyTxt));
        Invoke(nameof(HideNeedCardKeyTxt), 2f);
    }

    private void HideNeedCardKeyTxt()
    {
        needCardKeyText.gameObject.SetActive(false);
    }

    private void UpdateCardKeyText()
    {
        cardKeyText.text = $" × {cardKeyCount}";
    }
}
