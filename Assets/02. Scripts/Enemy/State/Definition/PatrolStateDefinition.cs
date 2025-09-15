using System;
using Constants;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolStateDefinition", menuName = "ScriptableObjects/PatrolStateDefinition")]
public class PatrolStateDefinition : StateDefinition
{
    public PatrolType patrolType;
    public float patrolPauseTime = 2f;

    public override Type StateType { get; } = typeof(PatrolState);
    public override IState CreateState(Enemy enemy)
                => new PatrolState(enemy, patrolType, patrolPauseTime);
}
