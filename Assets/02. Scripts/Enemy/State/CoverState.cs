using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoverStateDefinition", menuName = "ScriptableObjects/CoverStateDefinition")]
public class CoverState : BaseState
{
    public CoverState(Enemy owner) : base(owner)
    {
    }


    public override void Enter()
    {
        ConditionalLogger.Log("CoverState Enter");
    }


    public override void Exit()
    {
        ConditionalLogger.Log("CoverState Exit");
    }
}
