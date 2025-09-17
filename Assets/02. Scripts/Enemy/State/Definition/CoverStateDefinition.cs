using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CoverStateDefinition", menuName = "ScriptableObjects/StateDefinitions/CoverStateDefinition")]
public class CoverStateDefinition : StateDefinition
{
    public LayerMask obstacleLayer; // 엄폐물 레이어
    public LayerMask enemyLayer; // 적 레이어
    public float coverOffset = 2f; // 엄폐물에서 떨어진 거리

    public override Type StateType => typeof(CoverState);

    public override IState CreateState(Enemy owner)
        => new CoverState(owner, obstacleLayer, enemyLayer, coverOffset);
}
