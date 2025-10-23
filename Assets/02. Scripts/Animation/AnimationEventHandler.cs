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
        NoiseManager.Instance.EmitNoise(transform, transform.position, NoiseManager.Instance.WalkNoise);
    }
    public void OnRunStep()
    {
        CharacterSoundManager.Instance.PlayRunstepSound(transform.position);
        NoiseManager.Instance.EmitNoise(transform, transform.position, NoiseManager.Instance.RunNoise);
    }

    public void OnPunch()
    {
        PunchEvent?.Invoke();
    }
}
