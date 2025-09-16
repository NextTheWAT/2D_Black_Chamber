using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InvestigateStateDefinition", menuName = "ScriptableObjects/StateDefinitions/InvestigateStateDefinition")]
public class InvestigateStateDefinition : StateDefinition
{
    public float investigateDuration = 5f; // ���� ���� ���� �ð�
    public float investigateRange = 2f; // ���� �� �������� �̵��ϴ� ����
    public float pauseDuration = 1f; // ���� �� ���ߴ� �ð�

    public override Type StateType => typeof(InvestigateState);
    public override IState CreateState(Enemy enemy)
        => new InvestigateState(enemy, investigateDuration, investigateRange, pauseDuration);
}
