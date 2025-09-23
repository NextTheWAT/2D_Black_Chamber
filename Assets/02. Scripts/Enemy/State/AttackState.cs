using UnityEngine;

public class AttackState : BaseState
{
    private readonly float maxAttackRange = 6f; // 공격 범위
    private readonly float desiredAttackDistance = 3f; // 적과의 원하는 공격 거리
    private readonly float meleeAttackRange = 1.5f; // 근접 공격 범위


    public AttackState(Enemy owner, float maxAttackRange, float desiredAttackDistance, float meleeAttackRange) : base(owner)
    {
        this.maxAttackRange = maxAttackRange;
        this.desiredAttackDistance = desiredAttackDistance;
        this.meleeAttackRange = meleeAttackRange;
    }

    public bool IsTargetInAttackRange
    {
        get
        {
            if (!owner.HasTarget) return false;
            float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
            return distToTarget <= maxAttackRange;
        }
    }


    public bool IsTargetInDesiredAttackDistance
    {
        get
        {
            if (!owner.HasTarget) return false;
            float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
            return distToTarget <= desiredAttackDistance;
        }
    }

    public bool IsTargetInMeleeRange
    {
        get
        {
            if (!owner.HasTarget) return false;
            float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
            return distToTarget <= meleeAttackRange;
        }
    }

    private float meleeAttackCooldown = 1.0f;
    private float nextMeleeAttackTime = 0f;

    public override void Enter()
        => ConditionalLogger.Log("AttackState Enter");

    public override void Update()
    {
        if (!owner.HasTargetInFOV) return;

        GameManager.Instance.RefreshCombatTimer();
        owner.LookPoint = owner.Target.position;
        owner.Agent.isStopped = false;

        if (owner.Shooter.CurrentAmmo <= 0 && owner.Shooter.CurrentMagazine <= 0)
        {
            owner.AnimationController.SetActiveShoot(false);
            // 타겟이 근접 공격 범위 내에 있는지 확인
            if (IsTargetInMeleeRange)
            {
                if (Time.time < nextMeleeAttackTime) return;
                nextMeleeAttackTime = Time.time + meleeAttackCooldown;

                MeleeAttack();
                owner.Agent.isStopped = true;
                owner.AnimationController.SetActivePunch(true);
            }
            else
            {
                owner.MoveTo(owner.Target.position);
                owner.AnimationController.SetActivePunch(false);
            }
        }
        else
        {
            owner.AnimationController.SetActivePunch(false);
            // 타겟이 공격 범위 내에 있는지 확인
            if (IsTargetInAttackRange)
            {
                owner.Attack();
                owner.AnimationController.SetActiveShoot(true);
                if (!IsTargetInDesiredAttackDistance)
                    owner.MoveTo(owner.Target.position);
                else
                    owner.Agent.isStopped = true;
            }
            else
            {
                owner.MoveTo(owner.Target.position);
                owner.AnimationController.SetActiveShoot(false);
            }
        }
    }

    public void MeleeAttack()
    {
        ConditionalLogger.Log($"{owner.name} melee attack!");
    }

    public override void Exit()
    {
        ConditionalLogger.Log("AttackState Exit");
        owner.AnimationController.SetActiveShoot(false);
        owner.Agent.isStopped = false;
    }
}
