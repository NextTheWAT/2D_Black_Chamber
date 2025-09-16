using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FleeStateDefinition", menuName = "ScriptableObjects/StateDefinitions/FleeStateDefinition")]
public class FleeStateDefinition : StateDefinition
{
    public float fleeDistance = 10f;
    public float fleeDuration = 5f;

    public override Type StateType => typeof(FleeState);

    public override IState CreateState(Enemy owner)
        => new FleeState(owner, fleeDistance, fleeDuration);
}
