using System;
using System.Collections.Generic;

public static class StateFactory
{
    public static Dictionary<Type, IState> CreateStates(Enemy enemy)
    {
        var result = new Dictionary<Type, IState>();

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
