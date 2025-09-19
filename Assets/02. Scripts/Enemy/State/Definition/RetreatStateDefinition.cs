using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RetreatStateDefinition", menuName = "ScriptableObjects/StateDefinitions/RetreatStateDefinition")]
public class RetreatStateDefinition : StateDefinition
{
    [Range(0f, 1f)]
    public float retreatHealthRatio = 0.2f; // ÈÄÅğÇÏ´Â Ã¼·Â ºñÀ²
    public float retreatDistance = 5f; // ÈÄÅğ °Å¸®
    public float returnTime = 5f; // ÈÄÅğ ÈÄ º¹±Í ½Ã°£

    public override Type StateType => typeof(RetreatState);
    public override IState CreateState(Enemy enemy)
        => new RetreatState(enemy, retreatHealthRatio, retreatDistance, returnTime);

}
