using Constants;

public class TargetFSM : StateMachine
{
    public TargetFSM(Enemy owner, StateTable stateTable) : base(owner, stateTable)
    {
        FleeState fleeState = GetState<FleeState>();


        // Global
        AddGlobalTransition<FleeState>(() => owner.IsHit);

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasSuspiciousTarget); // 근처에 타겟 있으면 의심

        // Suspect
        AddTransition<SuspectState, PatrolState>(() => !owner.HasSuspiciousTarget); // 근처에 타겟 없으면 다시 순찰
        AddTransition<SuspectState, FleeState>(() => owner.HasTarget); // 타겟 발견하면 도망

        // Flee
        AddTransition<FleeState, PatrolState>(() => !fleeState.IsFleeing);

        // Return
        AddTransition<ReturnState, FleeState>(() => owner.HasTarget);
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived);
    }
}
