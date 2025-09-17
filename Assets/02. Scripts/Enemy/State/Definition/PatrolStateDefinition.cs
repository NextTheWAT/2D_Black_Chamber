using System;
using Constants;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolStateDefinition", menuName = "ScriptableObjects/StateDefinitions/PatrolStateDefinition")]
public class PatrolStateDefinition : StateDefinition
{
    public PatrolType patrolType; // 순찰 유형
    public float patrolPauseTime = 2f; // 순찰 지점에서 대기 시간
    public float fixedPatrolAngle = 180f; // 고정 순찰 시 회전 각도

    public override Type StateType => typeof(PatrolState);
    public override IState CreateState(Enemy enemy)
                => new PatrolState(enemy, patrolType, patrolPauseTime, fixedPatrolAngle);
}
