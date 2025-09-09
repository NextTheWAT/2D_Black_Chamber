using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState currentState;
    public IState CurrentState => currentState;

    private Dictionary<Type, IState> states = new();
    private Enemy owner;

    public StateMachine(Enemy owner)
    {
        this.owner = owner;

        AddState(new PatrolState(owner));
        AddState(new ChaseState(owner));
        // AddState(new AttackState(owner));
        // AddState(new ReturnState(owner));

        ChangeState<PatrolState>();
    }


    public void AddState<T>(T state) where T : IState
    {
        var type = typeof(T);
        if (states.ContainsKey(type))
        {
            ConditionalLogger.LogWarning($"�̹� �ش� {type}�� �����մϴ�.");
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

        currentState?.Exit();
        currentState = states[type];
        currentState.Enter();
    }

    public void UpdateState()
        => currentState?.Update();
}
