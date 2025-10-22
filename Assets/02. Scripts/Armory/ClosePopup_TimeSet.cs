using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosePopup_timeSet : MonoBehaviour
{
    public Button closeButton;



    private void Awake()
    {
        if (closeButton) closeButton.onClick.AddListener(CloseThisPopup);
    }

    public void CloseThisPopup()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
