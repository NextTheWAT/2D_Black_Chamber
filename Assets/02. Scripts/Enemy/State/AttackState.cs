using UnityEngine;

public class AttackState : BaseState
{
    private readonly float attackRange = 1.5f;

    public AttackState(Enemy owner, float attackRange) : base(owner)
    {
        this.attackRange = attackRange;
    }

    public bool IsTargetInAttackRange
    {
        get
        {
            if (!owner.HasTarget) return false;
            float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
            return distToTarget <= attackRange;
        }
    }

    public override void Enter()
    {
        ConditionalLogger.Log("AttackState Enter");
        owner.Target = GameManager.Instance.player;
        // owner.Agent.isStopped = true;
    }

    public override void Update()
    {
        owner.LookPoint = owner.Target.position;

        if (owner.IsTargetInSight)
        {
            owner.Attack();
            owner.AnimationController.SetActiveShoot(true);
        }
        else
        {
            owner.AnimationController.SetActiveShoot(false);
        }
    }

    public override void Exit()
    {
        ConditionalLogger.Log("AttackState Exit");
        owner.AnimationController.SetActiveShoot(false);
        // owner.Agent.isStopped = false;
    }
}
