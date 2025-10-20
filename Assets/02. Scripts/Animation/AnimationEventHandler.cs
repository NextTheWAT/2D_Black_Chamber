using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class AnimationEventHandler : MonoBehaviour
{
    public UnityEvent PunchEvent;

    public void OnWalkstep()
    {
        CharacterSoundManager.Instance.PlayWalkstepSound(transform.position);
    }
    public void OnRunStep()
    {
        CharacterSoundManager.Instance.PlayRunstepSound(transform.position);
    }

    public void OnPunch()
    {
        PunchEvent?.Invoke();
    }
}
