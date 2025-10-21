using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(Enemy owner) : base(owner) { }

    private float meleeAttackCooldown = 1.0f;
    private float nextMeleeAttackTime = 0f;

    public bool IsTargetInAttackRange
    {
        get
        {
            if (!owner.HasTarget) return false;
            float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
            return distToTarget <= owner.Data.rangedAttackRange;
        }
    }


    public bool IsTargetInDesiredAttackDistance
    {
        get
        {
            if (!owner.HasTarget) return false;
            float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
            return distToTarget <= owner.Data.desiredAttackDistance;
        }
    }

    public bool IsTargetInMeleeRange
    {
        get
        {
            if (!owner.HasTarget) return false;
            float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
            return distToTarget <= owner.Data.meleeAttackRange;
        }
    }

    public override void Enter()
        => ConditionalLogger.Log("AttackState Enter");

    public override void Update()
    {
        if (!owner.HasTargetInFOV) return;

        owner.LookPoint = owner.Target.position;
        owner.Agent.isStopped = false;

        // 탄약이 있으면 원거리 공격, 없으면 근접 공격
        if (owner.Shooter.HasAnyAmmo)
        {
            // 타겟이 공격 범위 내에 있는지 확인
            if (IsTargetInAttackRange)
            {
                owner.Attack();
                if (!IsTargetInDesiredAttackDistance)
                    owner.MoveTo(owner.Target.position);
                else
                    owner.Agent.isStopped = true;
            }
            else
            {
                owner.MoveTo(owner.Target.position);
            }
        }
        else
        {
            // 타겟이 근접 공격 범위 내에 있는지 확인
            if (IsTargetInMeleeRange)
            {
                if (Time.time < nextMeleeAttackTime) return;
                nextMeleeAttackTime = Time.time + meleeAttackCooldown;

                MeleeAttack();
                owner.Agent.isStopped = true;
                owner.AnimationController.PlayPunch();
            }
            else
            {
                owner.MoveTo(owner.Target.position);
            }
        }
    }

    public void MeleeAttack()
    {
        Health playerHealth = GameManager.Instance.Player.GetComponent<Health>();
        if (playerHealth)
            playerHealth.TakeDamage(owner.Data.meleeAttackDamage);

        ConditionalLogger.Log($"{owner.name} melee attack!");
    }

    public override void Exit()
    {
        ConditionalLogger.Log("AttackState Exit");
        owner.Agent.isStopped = false;
    }
}
