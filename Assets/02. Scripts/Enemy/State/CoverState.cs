using UnityEngine;
using UnityEngine.AI;
public class CoverState : BaseState
{
    // 1. 적이 플레이어로 이동하는 경로를 참조한다.
    // 2. 경로의 끝 지점부터 시작 지점까지의 위치를 임의의 점을 이동하며, 점과 플레이어 사이에 장애물이 충돌하는지 확인한다.
    // 3. 만약 충돌한다면 있다면 그 충돌 지점을 기반으로 삼고, 장애물의 벽에서 플레이어와 반대되는 일정 지점을 엄폐 지점으로 삼는다.
    // 4. 혹시 엄폐 지점에 다른 적이 있다면 이번 지점은 포기하고, 점을 계속 이동시켜 적절한 위치를 찾는다.
    // 5. 엄폐 할 곳이 없거나, 경로를 못찾으면 현재 위치에 머문다.
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

                // 벽 안쪽 깊이
                float coverDepth = owner.Agent.radius;

                // 벽 표면 tangent
                Vector2 tangent = new(-hit.normal.y, hit.normal.x);

                // 플레이어 반대 방향
                Vector2 dirToPlayer = ((Vector2)owner.Target.position - hit.point).normalized;
                Vector2 oppositeDir = -dirToPlayer;

                // 벽 안쪽으로 이동
                Vector2 inward = hit.point + hit.normal * coverDepth;

                // 벽을 따라 플레이어 반대 방향으로 이동
                Vector2 tangentMove = coverOffset * Vector2.Dot(oppositeDir, tangent) * tangent;
                Vector2 finalCoverPoint = inward + tangentMove;

                // NavMesh 위로 보정
                if (NavMesh.SamplePosition(finalCoverPoint, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
                    finalCoverPoint = navHit.position;

                // 주변 적과 겹치지 않도록 밀어내기
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
