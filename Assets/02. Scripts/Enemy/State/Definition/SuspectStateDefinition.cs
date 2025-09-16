using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SuspectStateDefinition", menuName = "ScriptableObjects/StateDefinitions/SuspectStateDefinition")]
public class SuspectStateDefinition : StateDefinition
{
    public float suspicionBuildTime = 3f; // �ǽ� ���°� �ִ�ġ�� �����ϴ� �ð�

    public override Type StateType { get; } = typeof(SuspectState);
    public override IState CreateState(Enemy enemy)
        => new SuspectState(enemy, suspicionBuildTime);
}
