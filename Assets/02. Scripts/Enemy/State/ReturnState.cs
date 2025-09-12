using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnState : BaseState
{
    public ReturnState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        ConditionalLogger.Log("ReturnState Enter");
        if (owner.PatrolPoints[owner.StartPatrolPointIndex])
            owner.MoveTo(owner.PatrolPoints[owner.StartPatrolPointIndex].position);
    }

    public override void Update()
    {
        owner.FindTarget();

        if (owner.HasTarget)
        {
            if(owner.isTarget)
                owner.ChangeState<FleeState>();
            else
                owner.ChangeState<ChaseState>();
            return;
        }

        if(owner.Agent.remainingDistance < 0.01f)
            owner.ChangeState<PatrolState>();
    }

    public override void Exit()
    {
        ConditionalLogger.Log("ReturnState Exit");
    }
}
