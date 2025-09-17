using UnityEngine;
using UnityEngine.AI;

public class CoverState : BaseState
{
    private LayerMask obstacleLayer;
    private LayerMask enemyLayer;
    private float coverOffset = 2f;
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
        Vector2 coverPoint = GetCoverPoint();
        if (Vector2.Distance(owner.transform.position, coverPoint) > 1f)
            owner.MoveTo(coverPoint);
    }

    public override void Update()
    {
        if (owner.IsArrived)
        {
            owner.LookAt(lookPoint);
        }
        try
        {
            Vector2 coverPoint = GetCoverPoint();
            if (Vector2.Distance(owner.transform.position, coverPoint) > 1f)
                owner.MoveTo(coverPoint);
        }
        catch (System.Exception ex)
        {
            ConditionalLogger.LogError($"CoverState Update Exception: {ex.Message}");
            return;
        }
    }

    public override void Exit()
    {
        ConditionalLogger.Log("CoverState Exit");
    }

    private Vector2 GetCoverPoint()
    {
        if (!owner.HasTarget) return owner.transform.position;

        Vector3[] corners = GetPathCorners();
        if (corners == null || corners.Length == 0)
            return owner.transform.position;

        for (int i = corners.Length - 1; i > 0; i--)
        {
            Vector2 start = corners[i];
            Vector2 end = corners[i - 1];
            float t = 0f;

            int count = 0;
            while (t < 1f)
            {
                if (count++ > 10)
                {
                    ConditionalLogger.LogError("GetCoverPoint: Too many iterations");
                    break;
                }
                t += 0.1f;
                Vector2 point = Vector2.Lerp(start, end, t);
                RaycastHit2D hit = Physics2D.Linecast(point, owner.Target.position, obstacleLayer);

                if (hit.collider)
                {
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
                    Vector2 tangentMove = tangent * Vector2.Dot(oppositeDir, tangent) * coverOffset;
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
        }


        /*
        Vector2? lastBlockedCorner = null;
        Vector2? lastOpenedCorner = null; 
        
        foreach (var corner in corners)
        {
            bool isBlocked = Physics2D.Linecast(corner, owner.Target.position, obstacleLayer);
            if (isBlocked) lastBlockedCorner = corner;
            else
            {
                lastOpenedCorner = corner;
                if (lastBlockedCorner == null) return owner.transform.position;
                break;
            }
        }

        if (!lastBlockedCorner.HasValue || !lastOpenedCorner.HasValue)
            return owner.transform.position;

        Vector2 start = lastOpenedCorner.Value;
        Vector2 end = lastBlockedCorner.Value;
        float t = 0f;

        lookPoint = start;

        while (t < 1f)
        {
            t += 0.1f;
            Vector2 point = Vector2.Lerp(start, end, t);
            RaycastHit2D hit = Physics2D.Linecast(point, owner.Target.position, obstacleLayer);

            if (hit.collider)
            {
                // 벽 안쪽 깊이
                float coverDepth = 0.75f;

                // 벽 표면 tangent
                Vector2 tangent = new(-hit.normal.y, hit.normal.x);

                // 플레이어 반대 방향
                Vector2 dirToPlayer = ((Vector2)owner.Target.position - hit.point).normalized;
                Vector2 oppositeDir = -dirToPlayer;

                // 벽 안쪽으로 이동
                Vector2 inward = hit.point + hit.normal * coverDepth;

                // 벽을 따라 플레이어 반대 방향으로 이동
                Vector2 tangentMove = tangent * Vector2.Dot(oppositeDir, tangent) * coverOffset;
                Vector2 finalCoverPoint = inward + tangentMove;

                // NavMesh 위로 보정
                if (NavMesh.SamplePosition(finalCoverPoint, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
                    finalCoverPoint = navHit.position;

                if(Physics2D.OverlapCircle(finalCoverPoint, owner.Agent.radius, enemyLayer) != null)
                    continue;

                return finalCoverPoint;
            }
        }
        */
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
