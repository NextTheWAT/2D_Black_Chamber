using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator upperAnimator;
    public Animator lowerAnimator;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        SetMoveSpeed(0f);
        PlayIdle();
        SetActiveShoot(false);
    }



    public void SetMoveSpeed(float speed)
    {
        int hash = AnimationHash.MoveSpeed;
        upperAnimator.SetFloat(hash, speed);
        lowerAnimator.SetFloat(hash, speed);
    }

    public void PlayIdle() => SetMove(0f);

    public void PlayWalk() => SetMove(0.5f);

    public void PlayRun() => SetMove(1f);

    public void SetMove(float speed)
    {
        int hash = AnimationHash.MoveBlend;
        upperAnimator.SetFloat(hash, speed);
        lowerAnimator.SetFloat(hash, speed);
    }

    public void SetActiveShoot(bool value)
    {
        int hash = AnimationHash.Shoot;
        upperAnimator.SetBool(hash, value);
    }

    public void PlayThrow()
    {
        int hash = AnimationHash.Throw;
        upperAnimator.SetTrigger(hash);
    }

    public void PlayDie()
    {
        int hash = AnimationHash.Die;
        upperAnimator.SetTrigger(hash);
        lowerAnimator.SetTrigger(hash);
    }

    public void PlayHit()
    {
        int hash = AnimationHash.Hit;
        upperAnimator.SetTrigger(hash);
    }

    public void PlayUse()
    {
        int hash = AnimationHash.Use;
        upperAnimator.SetTrigger(hash);
    }
    public void PlayPunch()
    {
        int hash = AnimationHash.Punch;
        upperAnimator.SetTrigger(hash);
    }

    public void PlayReload()
    {
        int hash = AnimationHash.Reload;
        upperAnimator.SetTrigger(hash);
    }

    public void PlaySwitch()
    {
        int hash = AnimationHash.Switch;
        upperAnimator.SetTrigger(hash);
    }

}
