using UnityEngine;

public class AttackState : BaseState
{
    private readonly float maxAttackRange = 6f; // 공격 범위
    private readonly float desiredAttackDistance = 3f; // 적과의 원하는 공격 거리
    private readonly int meleeAttackDamage = 10; // 근접 공격 데미지
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

        owner.LookPoint = owner.Target.position;
        owner.Agent.isStopped = false;

        // 탄약이 있으면 원거리 공격, 없으면 근접 공격
        if (owner.Shooter.HasAnyAmmo)
        {
            owner.AnimationController.SetActivePunch(false);
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
                owner.AnimationController.SetActivePunch(true);
            }
            else
            {
                owner.MoveTo(owner.Target.position);
                owner.AnimationController.SetActivePunch(false);
            }
        }
    }

    public void MeleeAttack()
    {
        Health playerHealth = GameManager.Instance.Player.GetComponent<Health>();
        if (playerHealth)
            playerHealth.TakeDamage(meleeAttackDamage);

        ConditionalLogger.Log($"{owner.name} melee attack!");
    }

    public override void Exit()
    {
        ConditionalLogger.Log("AttackState Exit");
        owner.Agent.isStopped = false;
    }
}
