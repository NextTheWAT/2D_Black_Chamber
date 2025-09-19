using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ChaseState : BaseState
{
    private readonly float chaseRange = 8f;

    public bool IsTargetInChaseRange
    {
        get
        {
            if (!owner.HasTarget) return false;
            float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
            return distToTarget <= chaseRange;
        }
    }

    public ChaseState(Enemy owner, float chaseRange) : base(owner)
    {
        this.chaseRange = chaseRange;
    }

    public override void Enter()
    {
        ConditionalLogger.Log("ChaseState Enter");
        owner.Target = GameManager.Instance.player;
    }

    public override void Update()
    {
        if (owner.HasTarget)
            owner.MoveTo(owner.Target.position);
    }

    public override void Exit()
    {
        ConditionalLogger.Log("ChaseState Exit");
    }

}
