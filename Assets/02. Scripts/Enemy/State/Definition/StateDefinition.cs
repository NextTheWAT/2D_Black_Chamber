using System;
using UnityEngine;

public abstract class StateDefinition : ScriptableObject
{
    public abstract Type StateType { get; }
    public abstract IState CreateState(Enemy enemy);
}
