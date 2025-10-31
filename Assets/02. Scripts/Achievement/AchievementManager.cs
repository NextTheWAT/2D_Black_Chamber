using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;


    [Header("¾÷Àû UI")]
    public List<Image> achievementsImages;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void UnlockAchievement(int index)
    {
        if (achievementsImages == null || index < 0 || index >= achievementsImages.Count)
            return;

            achievementsImages[index].color = Color.yellow;
    }
}
