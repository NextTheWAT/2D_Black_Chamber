using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class AttackState : BaseState
{
    public AttackState(Enemy owner) : base(owner) { }
    public override StateType StateType => StateType.Attack;

    public override void Enter()
    {
        ConditionalLogger.Log("AttackState Enter");
        owner.AnimationController.SetActiveShoot(true);
        owner.Agent.isStopped = true;
    }

    public override void Update()
    {
        owner.FindTarget();
        if (owner.HasTarget)
        {
            owner.MoveTo(owner.Target.position);
            owner.Attack();
        }
        else
            owner.ChangeState(StateType.Investigate);
    }

    public override void Exit()
    {
        ConditionalLogger.Log("AttackState Exit");
        owner.AnimationController.SetActiveShoot(false);
        owner.Agent.isStopped = false;
    }
}
