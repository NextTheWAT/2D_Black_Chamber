using System.Collections;
using UnityEngine;
using Constants;

public class InvestigateState : BaseState
{
    private readonly float startDelay = 3f; // ���� ���� �� ��� �ð�
    private readonly float investigateDuration = 5f; // ���� ���� ���� �ð�
    private readonly float investigateRange = 2f; // ���� �� �������� �̵��ϴ� ����
    private readonly float pauseDuration = 1f; // ���� �� ���ߴ� �ð�
    private float investigateTimer = 0f;
    public bool IsInvestigating => investigateTimer < investigateDuration || GameManager.Instance.IsCombat;

    private Coroutine investigateCoroutine;

    public InvestigateState(Enemy owner, float startDelay, float investigateDuration, float investigateRange, float pauseDuration) : base(owner)
    {
        this.startDelay = startDelay;
        this.investigateDuration = investigateDuration;
        this.investigateRange = investigateRange;
        this.pauseDuration = pauseDuration;
    }

    public override void Enter()
    {
        BeginInvestigate();
        ConditionalLogger.Log("InvestigateState Enter");
    }

    public override void Exit()
    {
        StopInvestigate();
        ConditionalLogger.Log("InvestigateState Exit");
    }

    private void BeginInvestigate()
    {
        if (investigateCoroutine != null)
            owner.StopCoroutine(investigateCoroutine);
        investigateCoroutine = owner.StartCoroutine(InvestigateLoop());

        owner.AutoRotate = true;
    }

    private void StopInvestigate()
    {
        if (investigateCoroutine != null)
        {
            owner.StopCoroutine(investigateCoroutine);
            investigateCoroutine = null;
        }

        owner.AutoRotate = false;
    }

    private IEnumerator InvestigateLoop()
    {
        // ���� ���� �� ���
        Vector2 dirToLastKnown = (owner.LastKnownTargetPos - (Vector2)owner.transform.position).normalized;
        owner.LookPoint = (Vector2)owner.transform.position + dirToLastKnown;
        owner.Agent.isStopped = true;
        yield return new WaitForSeconds(startDelay);
        owner.Agent.isStopped = false;

        // ó�� �÷��̾� ��ġ�� �̵�
        owner.MoveTo(owner.LastKnownTargetPos);
        investigateTimer = 0f;

        while (investigateTimer < investigateDuration)
        {
            if (owner.IsArrived) break;
            investigateTimer += Time.deltaTime;
            yield return null;
        }

        while (true)
        {
            // ������ ���� �������� �̵�
            do
            {
                owner.MoveTo(GetRandomInvestigatePoint());
                investigateTimer += Time.deltaTime;
                yield return null;
            }
            while (!owner.Agent.hasPath);

            // �������� ������ ������ ���
            while (!owner.IsArrived)
            {
                investigateTimer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(pauseDuration);
        }
    }

    // ������ ���� ���� ����
    private Vector2 GetRandomInvestigatePoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * investigateRange;
        Vector2 investigatePoint = (Vector2)owner.transform.position + randomDirection;
        return investigatePoint;
    }
}
