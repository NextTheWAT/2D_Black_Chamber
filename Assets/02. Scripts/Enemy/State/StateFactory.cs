using System;
using Constants;

public static class StateFactory
{
    public static IState CreateState(StateType stateType, Enemy owner)
    {
        return stateType switch
        {
            StateType.Patrol => new PatrolState(owner),
            StateType.Chase => new ChaseState(owner),
            StateType.Investigate => new InvestigateState(owner),
            StateType.Return => new ReturnState(owner),
            StateType.Attack => new AttackState(owner),
            _ => null,
        };
    }
}
