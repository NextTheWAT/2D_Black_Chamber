using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosePopup : MonoBehaviour
{
    public Button closeButton;



    private void Awake()
    {
        if (closeButton) closeButton.onClick.AddListener(CloseThisPopup);
    }

    public void CloseThisPopup()
    {
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);
        gameObject.SetActive(false);
    }
}
