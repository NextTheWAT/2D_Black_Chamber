using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public AudioClip[] clips;
    public float volume = 1f;
}

public class AnimationEventHandler : MonoBehaviour
{
    public AudioSource audioSource;
    public SoundData footstepSoundData;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponentInParent<AudioSource>();
    
    }

    public void OnFootstep()
    {
        if (audioSource != null && footstepSoundData != null)
        {
            AudioClip footstepClip = footstepSoundData.clips[Random.Range(0, footstepSoundData.clips.Length)];
            if (footstepClip == null) return;
            audioSource.volume = footstepSoundData.volume;
            audioSource.PlayOneShot(footstepClip);
        }
    }
}
