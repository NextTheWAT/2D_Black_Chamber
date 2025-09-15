using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class BaseState : IState
{
    protected Enemy owner;

    public BaseState(Enemy owner)
        => this.owner = owner;

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }
}
