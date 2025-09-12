using System;

public class Transition
{
    public IState FromState { get; }
    public IState ToState { get; }
    public Func<bool> Condition { get; }

    public Transition(IState from, IState to, Func<bool> condition)
    {
        FromState = from;
        ToState = to;
        Condition = condition;
    }
}