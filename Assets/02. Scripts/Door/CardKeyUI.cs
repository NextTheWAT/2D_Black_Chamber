using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardKeyUI : MonoBehaviour
{
    public static CardKeyUI Instance;
    public TextMeshProUGUI cardKeyText;
    private int cardKeyCount = 0;

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
    private void UpdateCardKeyText()
    {
        cardKeyText.text = $" ¡¿ {cardKeyCount}";
    }
}
