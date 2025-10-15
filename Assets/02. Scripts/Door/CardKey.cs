using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardKey : MonoBehaviour, Iinteraction
{
    public static bool hasCardKey = false;

    public void Interaction()
    {
        hasCardKey = true;
        gameObject.SetActive(false);
    }
}
