using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RetreatStateDefinition", menuName = "ScriptableObjects/StateDefinitions/RetreatStateDefinition")]
public class RetreatStateDefinition : StateDefinition
{
    [Range(0f, 1f)]
    public float retreatHealthRatio = 0.2f; // �����ϴ� ü�� ����
    public float retreatDistance = 5f; // ���� �Ÿ�

    public override Type StateType => typeof(RetreatState);
    public override IState CreateState(Enemy enemy)
        => new RetreatState(enemy, retreatHealthRatio, retreatDistance);

}
