using System.Collections;
using UnityEngine;
using Constants;

public class ReturnState : BaseState
{
    public ReturnState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        // '복귀' 대사 출력 로직 수정 (ScriptableObject 기반)
        // EnemyState.Return 상태의 랜덤 대사를 가져옴
        string dialogue = owner.GetRandomDialogue(EnemyState.Return);
        owner.DisplayDialogue(dialogue);

        ConditionalLogger.Log("ReturnState Enter");
        if (owner.ReturnPoint)
            owner.MoveTo(owner.ReturnPoint.position);
    }

    public override void Exit()
    {
        ConditionalLogger.Log("ReturnState Exit");
    }
}
