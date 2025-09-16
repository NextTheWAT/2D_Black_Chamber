using System;
using UnityEngine;


[CreateAssetMenu(fileName = "AssaultStateDefinition", menuName = "ScriptableObjects/StateDefinitions/AssaultStateDefinition")]
public class AssaultStateDefinition : StateDefinition
{
    public LayerMask targetLayer;

    public override Type StateType => typeof(AssaultState);
    public override IState CreateState(Enemy enemy)
            => new AssaultState(enemy, targetLayer);
}
