using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InvestigateStateDefinition", menuName = "ScriptableObjects/StateDefinitions/InvestigateStateDefinition")]
public class InvestigateStateDefinition : StateDefinition
{
    public float startDelay = 3f; // 조사 시작 전 대기 시간
    public float investigateDuration = 5f; // 조사 상태 지속 시간
    public float investigateRange = 2f; // 조사 중 무작위로 이동하는 범위
    public float pauseDuration = 1f; // 조사 중 멈추는 시간

    public override Type StateType => typeof(InvestigateState);
    public override IState CreateState(Enemy enemy)
        => new InvestigateState(enemy, startDelay, investigateDuration, investigateRange, pauseDuration);
}
