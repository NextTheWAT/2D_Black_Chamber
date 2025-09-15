using Constants;

public class TargetFSM : StateMachine
{
    public TargetFSM(Enemy owner, StateTable stateTable) : base(owner, stateTable)
    {
        FleeState fleeState = GetState<FleeState>();


        // Global
        AddGlobalTransition<FleeState>(() => owner.IsHit);

        // Patrol
        AddTransition<PatrolState, FleeState>(() => owner.HasTarget);

        // Flee
        AddTransition<FleeState, PatrolState>(() => !fleeState.IsFleeing);

        // Return
        AddTransition<ReturnState, FleeState>(() => owner.HasTarget);
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived);
    }
}
