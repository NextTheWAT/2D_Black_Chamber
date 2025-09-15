using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaseStateDefinition", menuName = "ScriptableObjects/ChaseStateDefinition")]
public class ChaseStateDefinition : StateDefinition
{
    public float initialChaseDelay = 2f; // 처음 타겟 발견했을 때 정지 시간
    public float investigateThreshold = 3f; // 몇 초 이상 못 보면 조사 상태로 전환

    public override Type StateType { get; } = typeof(ChaseState);
    public override IState CreateState(Enemy enemy)
        => new ChaseState(enemy, initialChaseDelay, investigateThreshold);
}
