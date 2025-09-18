using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RetreatStateDefinition", menuName = "ScriptableObjects/StateDefinitions/RetreatStateDefinition")]
public class RetreatStateDefinition : StateDefinition
{
    [Range(0f, 1f)]
    public float retreatHealthRatio = 0.2f; // 후퇴하는 체력 비율
    public float retreatDistance = 5f; // 후퇴 거리

    public override Type StateType => typeof(RetreatState);
    public override IState CreateState(Enemy enemy)
        => new RetreatState(enemy, retreatHealthRatio, retreatDistance);

}
