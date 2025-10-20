using System;
using System.Collections.Generic;

public static class StateFactory
{
    public static Dictionary<Type, IState> CreateStates(Enemy enemy)
    {
        var result = new Dictionary<Type, IState>();
        /*
        result.Add(typeof(PatrolState), new PatrolState(enemy));
        result.Add(typeof(SuspectState), new SuspectState(enemy));
        result.Add(typeof(InvestigateState), new InvestigateState(enemy));
        result.Add(typeof(AssaultState), new AssaultState(enemy));
        result.Add(typeof(CoverState), new CoverState(enemy));
        result.Add(typeof(AttackState), new AttackState(enemy));
        result.Add(typeof(DeathState), new DeathState(enemy));
        result.Add(typeof(RetreatState), new RetreatState(enemy));
        result.Add(typeof(ReturnState), new ReturnState(enemy));
        result.Add(typeof(FleeState), new FleeState(enemy));
        */
        
        AddState<PatrolState>(result, enemy);
        AddState<SuspectState>(result, enemy);
        AddState<InvestigateState>(result, enemy);
        AddState<AssaultState>(result, enemy);
        AddState<CoverState>(result, enemy);
        AddState<AttackState>(result, enemy);
        AddState<DeathState>(result, enemy);
        AddState<RetreatState>(result, enemy);
        AddState<ReturnState>(result, enemy);
        AddState<FleeState>(result, enemy);
        
        return result;
    }

    private static void AddState<T>(Dictionary<Type, IState> states, Enemy enemy) where T : IState
    {
        try
        {
            IState state = Activator.CreateInstance(typeof(T), new object[] { enemy }) as IState;
            
            if (state != null)
                states[state.GetType()] = state;
            else
                ConditionalLogger.LogWarning($"StateFactory에서 {typeof(T)} 상태를 생성하지 못했습니다.");
        }
        catch (Exception e)
        {
            ConditionalLogger.LogWarning($"StateFactory에서 {typeof(T)} 상태 생성 실패: {e.Message}");
        }
    }
}
