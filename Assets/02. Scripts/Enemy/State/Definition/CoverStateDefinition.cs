using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CoverStateDefinition", menuName = "ScriptableObjects/StateDefinitions/CoverStateDefinition")]
public class CoverStateDefinition : StateDefinition
{
    public LayerMask obstacleLayer; // ¾öÆó¹° ·¹ÀÌ¾î
    public float coverOffset = 2f; // ¾öÆó¹°¿¡¼­ ¶³¾îÁø °Å¸®

    public override Type StateType => typeof(CoverState);

    public override IState CreateState(Enemy owner)
        => new CoverState(owner, obstacleLayer, coverOffset);
}
