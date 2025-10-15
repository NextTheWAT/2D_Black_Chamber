using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardKey : MonoBehaviour, Iinteraction
{
    public static bool hasCardKey = false;

    public void Interaction()
    {
        Debug.Log("Ä«µåÅ°¸Ô±â");
        hasCardKey = true;
        gameObject.SetActive(false);
    }
}
