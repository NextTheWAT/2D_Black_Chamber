using UnityEngine;

public class AttackState : BaseState
{
    private readonly float maxAttackRange = 6f; // 공격 범위
    private readonly float desiredAttackDistance = 3f; // 적과의 원하는 공격 거리

    public AttackState(Enemy owner, float maxAttackRange, float desiredAttackDistance) : base(owner)
    {
        this.maxAttackRange = maxAttackRange;
        this.desiredAttackDistance = desiredAttackDistance;
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

    public override void Enter()
        => ConditionalLogger.Log("AttackState Enter");

    public override void Update()
    {

        if (owner.HasTargetInFOV)
        {
            owner.LookPoint = owner.Target.position;
            owner.Agent.isStopped = false;

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

    public override void Exit()
    {
        ConditionalLogger.Log("AttackState Exit");
        owner.AnimationController.SetActiveShoot(false);
        owner.Agent.isStopped = false;
    }
}
