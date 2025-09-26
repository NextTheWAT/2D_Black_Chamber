using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationEventHandler : MonoBehaviour
{
    public void OnWalkstep()
    {
        CharacterSoundManager.Instance.PlayWalkstepSound();
    }
    public void OnRunStep()
    {
        //NoiseManager.Instance.EmitNoise(transform.position, 4f);
        CharacterSoundManager.Instance.PlayRunstepSound();
    }
}
