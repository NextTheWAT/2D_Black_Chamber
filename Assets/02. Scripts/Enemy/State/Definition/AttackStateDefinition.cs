using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackStateDefinition", menuName = "ScriptableObjects/StateDefinitions/AttackStateDefinition")]
public class AttackStateDefinition : StateDefinition
{
    public float maxAttackRange = 6f;
    public float desiredAttackDistance = 3f;

    public override Type StateType => typeof(AttackState);
    public override IState CreateState(Enemy enemy)
        => new AttackState(enemy, maxAttackRange, desiredAttackDistance);
}
