using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaseStateDefinition", menuName = "ScriptableObjects/ChaseStateDefinition")]
public class ChaseStateDefinition : StateDefinition
{
    public float initialChaseDelay = 2f; // ó�� Ÿ�� �߰����� �� ���� �ð�
    public float investigateThreshold = 3f; // �� �� �̻� �� ���� ���� ���·� ��ȯ

    public override Type StateType { get; } = typeof(ChaseState);
    public override IState CreateState(Enemy enemy)
        => new ChaseState(enemy, initialChaseDelay, investigateThreshold);
}
