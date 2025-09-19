using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ReturnStateDefinition", menuName = "ScriptableObjects/StateDefinitions/ReturnStateDefinition")]
public class ReturnStateDefinition : StateDefinition
{
    public override Type StateType => typeof(ReturnState);
    public override IState CreateState(Enemy owner)
        => new ReturnState(owner);

}
