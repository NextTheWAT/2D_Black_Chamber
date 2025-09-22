using UnityEngine;
using Constants;

public class CharacterAnimationController : MonoBehaviour
{
    [Header("Animators (�ڵ� �˻� ����)")]
    public Animator upperAnimator;   // ������ Awake���� "UpperBody" �ڽĿ��� �ڵ� �Ҵ�
    public Animator lowerAnimator;   // ������ Awake���� "LowerBody" �ڽĿ��� �ڵ� �Ҵ�

    [Header("UpperBody Controllers")]
    [SerializeField] private RuntimeAnimatorController upperPistolController;
    [SerializeField] private RuntimeAnimatorController upperRifleController;

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
        SetActiveShoot(false);
        SetActiveUse(false);
        SetActivePunch(false);
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

    public void SetActiveShoot(bool v) { if (upperAnimator) upperAnimator.SetBool(AnimationHash.Shoot, v); }
    public void SetActiveUse(bool v) { if (upperAnimator) upperAnimator.SetBool(AnimationHash.Use, v); }
    public void SetActivePunch(bool v) { if (upperAnimator) upperAnimator.SetBool(AnimationHash.Punch, v); }

    public void PlayThrow() { if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Throw); }
    public void PlayDie() { if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Die); }
    public void PlayHit() { if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Hit); }

    public void PlayReload()
    {
        if (upperAnimator) upperAnimator.SetTrigger(AnimationHash.Reload);
        WeaponSoundManager.Instance?.PlayReloadSound();
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
        SetActiveShoot(s.shoot);
        SetActiveUse(s.use);
        SetActivePunch(s.punch);
    }

    /// <summary>
    /// ���� Ÿ�Կ� ���� "��ü" Animator Controller�� ��ü.
    /// (��ü�� �ϳ��� ���)
    /// </summary>
    public void ApplyUpperWeaponAnimator(WeaponType type, bool playSwitchAnim = true)
    {
        if (!upperAnimator) return;

        if (playSwitchAnim) PlaySwitch();

        var snap = TakeSnapshot();

        var next = (type == WeaponType.Rifle) ? upperRifleController : upperPistolController;
        if (next != null && upperAnimator.runtimeAnimatorController != next)
        {
            upperAnimator.runtimeAnimatorController = next;
            upperAnimator.Update(0f); // ��� �ݿ�
        }

        Restore(snap);
    }
}
