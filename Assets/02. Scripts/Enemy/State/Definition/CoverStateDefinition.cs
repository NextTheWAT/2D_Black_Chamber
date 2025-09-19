using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CoverStateDefinition", menuName = "ScriptableObjects/StateDefinitions/CoverStateDefinition")]
public class CoverStateDefinition : StateDefinition
{
    public LayerMask obstacleLayer; // ���� ���̾�
    public LayerMask enemyLayer; // �� ���̾�
    public float coverOffset = 2f; // ���󹰿��� ������ �Ÿ�

    public override Type StateType => typeof(CoverState);

    public override IState CreateState(Enemy owner)
        => new CoverState(owner, obstacleLayer, enemyLayer, coverOffset);
}
