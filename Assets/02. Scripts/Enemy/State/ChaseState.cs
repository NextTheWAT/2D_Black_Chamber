using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    public ChaseState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        if (owner.Target == null)
        {
            owner.ChangeState<PatrolState>();
            return;
        }

        ConditionalLogger.Log("ChaseState Enter");
        owner.Agent.isStopped = false;
        owner.MoveTo(owner.Target.position);
    }
    public override void Update()
    {
        owner.FindTargetInFOV();

        if (owner.HasTarget)
        {
            owner.MoveTo(owner.Target.position);
        }
        else
        {
            owner.ChangeState<PatrolState>();
            return;
        }
    }

    public override void Exit()
    {
        ConditionalLogger.Log("ChaseState Exit");
        owner.Agent.isStopped = true;
    }

}
