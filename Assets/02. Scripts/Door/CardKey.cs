using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardKey : MonoBehaviour, Iinteraction
{
    public static bool hasCardKey = false;

    public void Interaction(Transform interactor)
    {
        Debug.Log("ī��Ű�Ա�");
        hasCardKey = true;
        gameObject.SetActive(false);
    }
}
