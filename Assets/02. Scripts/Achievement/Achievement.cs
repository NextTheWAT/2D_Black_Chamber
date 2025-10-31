using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achievement
{
    public string name;
    public string description;
    public bool isUnlocked = false;
    public Sprite icon;
}