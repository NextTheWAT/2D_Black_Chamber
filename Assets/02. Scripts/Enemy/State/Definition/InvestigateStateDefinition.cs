using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InvestigateStateDefinition", menuName = "ScriptableObjects/InvestigateStateDefinition")]
public class InvestigateStateDefinition : StateDefinition
{
    public float investigateDuration = 5f; // 조사 상태 지속 시간
    public float investigateRange = 2f; // 조사 중 무작위로 이동하는 범위
    public float pauseDuration = 1f; // 조사 중 멈추는 시간

    public override Type StateType { get; } = typeof(InvestigateState);
    public override IState CreateState(Enemy enemy)
        => new InvestigateState(enemy, investigateDuration, investigateRange, pauseDuration);
}
