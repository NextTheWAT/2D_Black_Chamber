using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : BaseState
{
    private Coroutine fleeCoroutine;

    public FleeState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        owner.Agent.isStopped = false;

        if (owner.HasTarget)
        {
            Vector2 pos = GetFleePoint();
            owner.MoveTo(pos);
        }

        ConditionalLogger.Log($"FleeState: Enter");
    }

    public override void Exit()
    {
        ConditionalLogger.Log($"FleeState: Enter");
    }


    Vector2 GetFleePoint()
    {
        Vector2 dirToPlayer = (owner.transform.position - owner.Target.position).normalized;
        return (Vector2)owner.transform.position + dirToPlayer * owner.FleeDistance;
    }
}
