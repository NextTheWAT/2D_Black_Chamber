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
        CharacterSoundManager.Instance.PlayRunstepSound();
    }
}
