using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ReturnStateDefinition", menuName = "ScriptableObjects/ReturnStateDefinition")]
public class ReturnStateDefinition : StateDefinition
{
    public override Type StateType { get; } = typeof(ReturnState);
    public override IState CreateState(Enemy owner)
        => new ReturnState(owner);

}
