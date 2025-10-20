using UnityEngine;

public class FleeState : BaseState
{
    public float fleeTimer;

    public bool IsFleeing => fleeTimer < owner.Data.fleeDuration || GameManager.Instance.IsCombat;

    public FleeState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        ConditionalLogger.Log($"FleeState: Enter");
        owner.MoveTo(GetFleePoint());
        fleeTimer = 0f;
    }

    public override void Update()
    {
        if (owner.HasTargetInFOV)
        {
            fleeTimer = 0f;
            owner.MoveTo(GetFleePoint());
        }
        else
        {
            if(!GameManager.Instance.IsCombat)
                fleeTimer += Time.deltaTime;
        }
    }

    public override void Exit()
    {
        ConditionalLogger.Log($"FleeState: Enter");
    }


    Vector2 GetFleePoint()
    {
        Vector2 dirToPlayer = ((Vector2)owner.transform.position - owner.LastKnownTargetPos).normalized;
        return (Vector2)owner.transform.position + dirToPlayer * owner.Data.fleeDistance;
    }
}
