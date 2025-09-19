using UnityEngine;
using UnityEngine.AI;
public class CoverState : BaseState
{
    // 1. ���� �÷��̾�� �̵��ϴ� ��θ� �����Ѵ�.
    // 2. ����� �� �������� ���� ���������� ��ġ�� ������ ���� �̵��ϸ�, ���� �÷��̾� ���̿� ��ֹ��� �浹�ϴ��� Ȯ���Ѵ�.
    // 3. ���� �浹�Ѵٸ� �ִٸ� �� �浹 ������ ������� ���, ��ֹ��� ������ �÷��̾�� �ݴ�Ǵ� ���� ������ ���� �������� ��´�.
    // 4. Ȥ�� ���� ������ �ٸ� ���� �ִٸ� �̹� ������ �����ϰ�, ���� ��� �̵����� ������ ��ġ�� ã�´�.
    // 5. ���� �� ���� ���ų�, ��θ� ��ã���� ���� ��ġ�� �ӹ���.
    private readonly LayerMask obstacleLayer;
    private readonly LayerMask enemyLayer;
    private readonly float coverOffset = 2f;
    private Vector2 lookPoint;

    public CoverState(Enemy owner, LayerMask obstacleLayer, LayerMask enemyLayer, float coverOffset) : base(owner)
    {
        this.obstacleLayer = obstacleLayer;
        this.enemyLayer = enemyLayer;
        this.coverOffset = coverOffset;
    }

    public override void Enter()
    {
        ConditionalLogger.Log("CoverState Enter");
        owner.Target = GameManager.Instance.player;
    }

    public override void Update()
    {
        Vector2 coverPoint = GetCoverPoint();
        if (Vector2.Distance(owner.transform.position, coverPoint) > 1f)
        {
            owner.MoveTo(coverPoint);
            owner.Agent.isStopped = false;

        }
        else
        {
            owner.Agent.isStopped = true;
        }

        if (owner.IsArrived)
            owner.LookPoint = lookPoint;
    }

    public override void Exit()
        => ConditionalLogger.Log("CoverState Exit");

    private Vector2 GetCoverPoint()
    {
        if (!owner.HasTarget) return owner.transform.position;

        Vector3[] corners = GetPathCorners();
        if (corners == null || corners.Length <= 2)
        {
            ConditionalLogger.Log("CoverState: No Path or Too Short");
            lookPoint = owner.Target.position;
            return owner.transform.position;
        }

        for (int i = corners.Length - 1; i > 0; i--)
        {
            Vector2 start = corners[i];
            Vector2 end = corners[i - 1];

            for (float t = 0.1f; t <= 1f; t += 0.1f)
            {
                Vector2 point = Vector2.Lerp(start, end, t);
                RaycastHit2D hit = Physics2D.Linecast(point, owner.Target.position, obstacleLayer);
                if (!hit.collider) continue;

                // �� ���� ����
                float coverDepth = owner.Agent.radius;

                // �� ǥ�� tangent
                Vector2 tangent = new(-hit.normal.y, hit.normal.x);

                // �÷��̾� �ݴ� ����
                Vector2 dirToPlayer = ((Vector2)owner.Target.position - hit.point).normalized;
                Vector2 oppositeDir = -dirToPlayer;

                // �� �������� �̵�
                Vector2 inward = hit.point + hit.normal * coverDepth;

                // ���� ���� �÷��̾� �ݴ� �������� �̵�
                Vector2 tangentMove = coverOffset * Vector2.Dot(oppositeDir, tangent) * tangent;
                Vector2 finalCoverPoint = inward + tangentMove;

                // NavMesh ���� ����
                if (NavMesh.SamplePosition(finalCoverPoint, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
                    finalCoverPoint = navHit.position;

                // �ֺ� ���� ��ġ�� �ʵ��� �о��
                if (Physics2D.OverlapCircle(finalCoverPoint, owner.Agent.radius, enemyLayer) != null)
                    continue;

                lookPoint = start;
                return finalCoverPoint;
            }
        }

        return owner.transform.position;
    }

    private Vector3[] GetPathCorners()
    {
        NavMeshPath path = new();
        if (NavMesh.CalculatePath((Vector2)owner.transform.position, (Vector2)owner.Target.position, NavMesh.AllAreas, path))
            return path.corners;
        return null;
    }

}
