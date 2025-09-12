using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class SoliderFSM : StateMachine
{
    public SoliderFSM(Enemy owner, StateTable stateTable) : base(owner, stateTable)
    {
        // Patrol
        AddTransition(StateType.Patrol, StateType.Chase, () => owner.HasTarget);

        // Chase
        AddTransition(StateType.Chase, StateType.Patrol, () => !owner.HasTarget);
        AddTransition(StateType.Chase, StateType.Attack, () => owner.HasTarget && owner.IsTargetInAttackRange);
        AddTransition(StateType.Chase, StateType.Investigate, () => owner.IsChaseToInvestigate);

        // Attack
        AddTransition(StateType.Attack, StateType.Investigate, () => !owner.HasTarget);

        // Investigate
        AddTransition(StateType.Investigate, StateType.Chase, () => owner.HasTarget);
        AddTransition(StateType.Investigate, StateType.Return, () => owner.IsInvestigateStop);

        // Return
        AddTransition(StateType.Return, StateType.Chase, () => owner.HasTarget);
        AddTransition(StateType.Return, StateType.Patrol, () => owner.IsArrived);

    }
}
