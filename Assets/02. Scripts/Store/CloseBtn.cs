using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseBtn : MonoBehaviour
{
    public GameObject canvasRoot;

    public void CloseCanvas()
    {
        if (canvasRoot != null)
            canvasRoot.SetActive(false);
    }
}
