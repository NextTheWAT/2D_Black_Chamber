using UnityEngine;

[CreateAssetMenu(menuName = "Sound/Sound Data")]
public class SoundData : ScriptableObject
{
    [Header("Sound")]
    public AudioClip[] clips;
    public float volume = 1f;
}
