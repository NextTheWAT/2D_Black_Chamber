using UnityEngine;
using Constants;

public class FleeState : BaseState
{
    private readonly float fleeDistance = 10f;
    private readonly float fleeDuration = 5f;
    public float fleeTimer;

    public bool IsFleeing => fleeTimer < fleeDuration;

    public FleeState(Enemy owner, float fleeDistance, float fleeDuration) : base(owner)
    {
        this.fleeDistance = fleeDistance;
        this.fleeDuration = fleeDuration;
    }

    public override void Enter()
    {
        ConditionalLogger.Log($"FleeState: Enter");
        owner.MoveTo(GetFleePoint());
        fleeTimer = 0f;
    }

    public override void Update()
    {
        owner.FindTarget();

        if (owner.HasTarget)
        {
            fleeTimer = 0f;
            owner.MoveTo(GetFleePoint());
        }
        else
            fleeTimer += Time.deltaTime;
    }

    public override void Exit()
    {
        ConditionalLogger.Log($"FleeState: Enter");
    }


    Vector2 GetFleePoint()
    {
        Vector2 dirToPlayer = ((Vector2)owner.transform.position - owner.LastKnownTargetPos).normalized;
        return (Vector2)owner.transform.position + dirToPlayer * fleeDistance;
    }
}
