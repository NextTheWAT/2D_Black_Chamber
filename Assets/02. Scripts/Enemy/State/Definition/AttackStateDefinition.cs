using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackStateDefinition", menuName = "ScriptableObjects/AttackStateDefinition")]
public class AttackStateDefinition : StateDefinition
{
    public float attackRange = 1.5f;

    public override Type StateType { get; } = typeof(AttackState);
    public override IState CreateState(Enemy enemy)
        => new AttackState(enemy, attackRange);
}
