using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SuspectStateDefinition", menuName = "ScriptableObjects/StateDefinitions/SuspectStateDefinition")]
public class SuspectStateDefinition : StateDefinition
{
    public float suspicionBuildTime = 3f; // 의심 상태가 최대치에 도달하는 시간

    public override Type StateType { get; } = typeof(SuspectState);
    public override IState CreateState(Enemy enemy)
        => new SuspectState(enemy, suspicionBuildTime);
}
