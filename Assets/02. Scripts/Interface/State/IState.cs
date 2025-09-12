using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public interface IState
{
    void Enter();
    void Exit();
    void Update();
    public StateType StateType { get; }
}
