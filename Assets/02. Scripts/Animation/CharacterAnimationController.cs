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
        SetMoveBlend(0f);
        SetMoveSpeed(1f);
        SetActiveShoot(false);
    }

    public void SetLowerBodyRotation(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;
        lowerAnimator.transform.right = direction;
        // lowerAnimator.transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    public void SetMoveSpeed(float speed)
    {
        int hash = AnimationHash.MoveSpeed;
        upperAnimator.SetFloat(hash, speed);
        lowerAnimator.SetFloat(hash, speed);
    }

    public void PlayIdle() => SetMoveBlend(0f);

    public void PlayWalk() => SetMoveBlend(0.5f);

    public void PlayRun() => SetMoveBlend(1f);

    public void SetMoveBlend(float blend)
    {
        int hash = AnimationHash.MoveBlend;
        upperAnimator.SetFloat(hash, blend);
        lowerAnimator.SetFloat(hash, blend);
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
