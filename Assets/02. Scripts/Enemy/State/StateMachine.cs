using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState currentState;
    public IState CurrentState => currentState;
    public Type CurrentStateType => currentState?.GetType();

    private Dictionary<Type, IState> states = new();
    private Enemy owner;

    public StateMachine(Enemy owner, StateTable stateTable)
    {
        this.owner = owner;

        foreach (var stateType in stateTable.stateTypes)
        {
            var state = StateFactory.CreateState(stateType, owner);
            if (state != null)
                AddState(state);
            else
                ConditionalLogger.LogWarning($"StateFactory에서 {stateType} 상태를 생성하지 못했습니다.");
        }

        ChangeState<PatrolState>();
    }

    public void AddState(IState state)
    {
        var type = state.GetType(); // 실제 구체 타입
        if (states.ContainsKey(type))
        {
            ConditionalLogger.LogWarning($"이미 {type} 상태가 존재합니다.");
            return;
        }

        states[type] = state;
    }


    public void ChangeState<T>() where T : IState
    {
        var type = typeof(T);
        if (!states.ContainsKey(type))
        {
            ConditionalLogger.LogWarning($"해당 {type}가 존재하지 않습니다.");
            return;
        }

        if(currentState != null && currentState.GetType() == type) return;

        currentState?.Exit();
        currentState = states[type];
        currentState.Enter();
    }

    public void UpdateState()
        => currentState?.Update();
}
