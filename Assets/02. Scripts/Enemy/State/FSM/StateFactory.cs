using System;
using System.Collections.Generic;
using Constants;

public static class StateFactory
{
    public static Dictionary<Type, IState> CreateStates(Enemy enemy, StateTable table)
    {
        var result = new Dictionary<Type, IState>();
        foreach (var definition in table.definitions)
        {
            var state = definition.CreateState(enemy);
            if (state != null)
                result[state.GetType()] = state;
            else
                ConditionalLogger.LogWarning($"StateFactory에서 {definition} 상태를 생성하지 못했습니다.");
        }
        return result;
    }
}
