using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    private float initialChaseDelay = 2f; // ó�� Ÿ�� �߰����� �� ���� �ð�
    private float investigateThreshold = 3f; // �� �� �̻� �� ���� ���� ���·� ��ȯ
    private Coroutine chaseCoroutine;


    public ChaseState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        if (owner.Target == null)
        {
            owner.ChangeState<PatrolState>();
            return;
        }

        ConditionalLogger.Log("ChaseState Enter");
        BeginChase();
    }

    public override void Exit()
    {
        ConditionalLogger.Log("ChaseState Exit");
        StopChase();
    }

    private void BeginChase()
    {
        if (chaseCoroutine != null)
            owner.StopCoroutine(chaseCoroutine);
        chaseCoroutine = owner.StartCoroutine(ChaseLoop());
    }

    private void StopChase()
    {
        if (chaseCoroutine != null)
        {
            owner.StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }
    }

    private IEnumerator ChaseLoop()
    {
        // �߰� ����Ʈ ����
        owner.Agent.isStopped = true;
        ConditionalLogger.Log("Find Enemy!");
        float chaseTimer = 0f;
        Vector2 lastTargetPos = owner.Target.position;
        
        while (chaseTimer < initialChaseDelay)
        {
            owner.FindTarget();

            if (owner.HasTarget)
                lastTargetPos = owner.Target.position;

            owner.MoveTo(lastTargetPos);
            chaseTimer += Time.deltaTime;
            yield return null;
        }

        ConditionalLogger.Log("Start Chase!");
        owner.Agent.isStopped = false;
        float investigateTimer = 0f;

        while (true)
        {
            owner.FindTarget();

            if (owner.HasTarget)
            {
                investigateTimer = 0;
                owner.MoveTo(owner.Target.position);
            }
            else
            {
                investigateTimer += Time.deltaTime;

                if (investigateTimer >= investigateThreshold)
                {
                    owner.ChangeState<InvestigateState>();
                    yield break;
                }
            }
            yield return null;
        }
    }

}
