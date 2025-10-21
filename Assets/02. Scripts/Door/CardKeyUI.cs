using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardKeyUI : MonoBehaviour
{
    public static CardKeyUI Instance;

    public GameObject cardKeyImage;
    public GameObject nocardKeyImage;
    public TextMeshProUGUI noKeyText;

    public TextMeshProUGUI needCardKeyText;

    private bool hasCardKey = false;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UpdateCardKeyUI(false);
    }
    public void AddCardKey()
    {
        UpdateCardKeyUI(true);
    }

    //public void UseCardKey()
    //{
    //    UpdateCardKeyUI(false);
    //}

    private void UpdateCardKeyUI(bool hasCardKey)
    {
        cardKeyImage.SetActive(hasCardKey);
        nocardKeyImage.SetActive(!hasCardKey);

        noKeyText.gameObject.SetActive(!hasCardKey);
    }

    public void NeedCardKeyTxt()
    {
        needCardKeyText.text = "카드키 필요!";
        needCardKeyText.gameObject.SetActive(true);

        CancelInvoke(nameof(HideNeedCardKeyTxt));
        Invoke(nameof(HideNeedCardKeyTxt), 1f);
    }

    private void HideNeedCardKeyTxt()
    {
        needCardKeyText.gameObject.SetActive(false);
    }
}
