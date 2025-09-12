using UnityEngine;
using Constants;

public class FleeState : BaseState
{
    public FleeState(Enemy owner) : base(owner) { }
    public override StateType StateType => StateType.Flee;

    public override void Enter()
    {
        ConditionalLogger.Log($"FleeState: Enter");
        owner.MoveTo(GetFleePoint());
        owner.fleeTimer = 0f;
    }

    public override void Update()
    {
        owner.FindTarget();

        if (owner.HasTarget)
        {
            owner.fleeTimer = 0f;
            owner.MoveTo(GetFleePoint());
        }
        else
        {
            owner.fleeTimer += Time.deltaTime;
        }
    }

    public override void Exit()
    {
        ConditionalLogger.Log($"FleeState: Enter");
    }


    Vector2 GetFleePoint()
    {
        Vector2 dirToPlayer = ((Vector2)owner.transform.position - owner.LastKnownTargetPos).normalized;
        return (Vector2)owner.transform.position + dirToPlayer * owner.FleeDistance;
    }
}
