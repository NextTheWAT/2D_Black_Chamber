using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaseStateDefinition", menuName = "ScriptableObjects/StateDefinitions/ChaseStateDefinition")]
public class ChaseStateDefinition : StateDefinition
{
    public float chaseRange = 8f; // 추격 거리 

    public override Type StateType => typeof(ChaseState);
    public override IState CreateState(Enemy enemy)
        => new ChaseState(enemy, chaseRange);
}
