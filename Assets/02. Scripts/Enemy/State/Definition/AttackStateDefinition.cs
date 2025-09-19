using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackStateDefinition", menuName = "ScriptableObjects/StateDefinitions/AttackStateDefinition")]
public class AttackStateDefinition : StateDefinition
{
    public float attackRange = 1.5f;

    public override Type StateType => typeof(AttackState);
    public override IState CreateState(Enemy enemy)
        => new AttackState(enemy, attackRange);
}
