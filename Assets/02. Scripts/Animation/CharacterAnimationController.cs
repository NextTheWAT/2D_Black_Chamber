using UnityEngine;
using Constants;

public class CharacterAnimationController : MonoBehaviour
{
    [Header("Animators (�ڵ� �˻� ����)")]
    public Animator upperAnimator;   // ������ Awake���� "UpperBody" �ڽĿ��� �ڵ� �Ҵ�
    public Animator lowerAnimator;   // ������ Awake���� "LowerBody" �ڽĿ��� �ڵ� �Ҵ�

    private void Awake()
    {
        AutoWireIfNull();   // UpperBody / LowerBody �ڵ� ����
        Initialize();
    }

    private void AutoWireIfNull()
    {
        if (upperAnimator == null)
            upperAnimator = transform.Find("UpperBody")?.GetComponent<Animator>();

        if (lowerAnimator == null)
            lowerAnimator = transform.Find("LowerBody")?.GetComponent<Animator>();
    }

    public void Initialize()
    {
        SetMoveBlend(0f);
        SetMoveSpeed(1f);
        SetActiveUse(false);
    }

    public void SetLowerBodyRotation(Vector2 direction)
    {
        if (!lowerAnimator) return;
        if (direction.sqrMagnitude < 0.01f) return;
        lowerAnimator.transform.right = direction;
    }

    public void SetMoveSpeed(float speed)
    {
        if (upperAnimator) upperAnimator.SetFloat(AnimationHash.MoveSpeed, speed);
        if (lowerAnimator) lowerAnimator.SetFloat(AnimationHash.MoveSpeed, speed);
    }

    public void PlayIdle() => SetMoveBlend(0f);
    public void PlayWalk() => SetMoveBlend(0.5f);
    public void PlayRun() => SetMoveBlend(1f);

    public void SetMoveBlend(float blend)
    {
        if (upperAnimator) upperAnimator.SetFloat(AnimationHash.MoveBlend, blend);
        if (lowerAnimator) lowerAnimator.SetFloat(AnimationHash.MoveBlend, blend);
    }

    public void SetActiveUse(bool v) { if (upperAnimator) upperAnimator.SetBool(AnimationHash.Use, v); }
    // public void SetActivePunch(bool v) { if (upperAnimator) upperAnimator.SetBool(AnimationHash.Punch, v); }
    public void PlayPunch() { if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Punch); }

    public void PlayShoot() { if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Shoot); }
    public void PlayThrow() { if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Throw); }
    public void PlayDie() { if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Die); }
    public void PlayHit() { if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Hit); }

    public void PlayReload()
    {
        if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Reload);
        WeaponSoundManager.Instance?.PlayReloadSound(transform.position);
    }

    public void PlaySwitch()
    {
        if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Switch);
    }

    // ---- Upper ��Ʈ�ѷ� ���� ��/�� ���� ������ ----
    private struct AnimSnapshot
    {
        public float blend, speed;
        public bool shoot, use, punch;
    }

    private AnimSnapshot TakeSnapshot()
    {
        var s = new AnimSnapshot();
        if (upperAnimator)
        {
            s.blend = upperAnimator.GetFloat(AnimationHash.MoveBlend);
            s.speed = upperAnimator.GetFloat(AnimationHash.MoveSpeed);
            s.shoot = upperAnimator.GetBool(AnimationHash.Shoot);
            s.use = upperAnimator.GetBool(AnimationHash.Use);
            s.punch = upperAnimator.GetBool(AnimationHash.Punch);
        }
        return s;
    }

    private void Restore(AnimSnapshot s)
    {
        SetMoveBlend(s.blend);
        SetMoveSpeed(s.speed);
        SetActiveUse(s.use);
    }

    /// <summary>
    /// ���� Ÿ�Կ� ���� "��ü" Animator Controller�� ��ü.
    /// (��ü�� �ϳ��� ���)
    /// </summary>
    public void ApplyUpperWeaponAnimator(GunData gunData, bool playSwitchAnim = true)
    {
        if (!upperAnimator) return;

        if (playSwitchAnim) PlaySwitch();

        var snap = TakeSnapshot();

        upperAnimator.runtimeAnimatorController = gunData.upperAnimator;
        upperAnimator.Update(0f); // ��� �ݿ�

        Restore(snap);
    }
}
