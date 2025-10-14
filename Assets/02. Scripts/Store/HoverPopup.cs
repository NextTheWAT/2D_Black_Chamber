using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverPopup : MonoBehaviour
{
    public static HoverPopup Instance;

    public GameObject HoverPanel;
    public Text itemNameText;
    public Text itemDescriptionText;

    private void Awake()
    {
        Instance = this;
        HoverPanel.SetActive(false);
    }

    public void ShowHoverPopup(string name, string description, Vector3 position)
    {
        itemNameText.text = name;
        itemDescriptionText.text = description;

        HoverPanel.transform.position = Input.mousePosition;   // ���콺��ġ�� �˾��� �߰�
        HoverPanel.SetActive(true);
    }

    public void HideHoverPopup()
    {
        HoverPanel.SetActive(false);
    }
}