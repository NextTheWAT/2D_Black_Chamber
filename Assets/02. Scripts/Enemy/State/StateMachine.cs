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
                ConditionalLogger.LogWarning($"StateFactory���� {stateType} ���¸� �������� ���߽��ϴ�.");
        }

        ChangeState<PatrolState>();
    }

    public void AddState(IState state)
    {
        var type = state.GetType(); // ���� ��ü Ÿ��
        if (states.ContainsKey(type))
        {
            ConditionalLogger.LogWarning($"�̹� {type} ���°� �����մϴ�.");
            return;
        }

        states[type] = state;
    }


    public void ChangeState<T>() where T : IState
    {
        var type = typeof(T);
        if (!states.ContainsKey(type))
        {
            ConditionalLogger.LogWarning($"�ش� {type}�� �������� �ʽ��ϴ�.");
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
