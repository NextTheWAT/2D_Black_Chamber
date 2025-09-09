using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FOVUtility
{
    public static bool IsCircleVisible(this Transform origin, CircleCollider2D circle, float viewAngle, float viewDistance, LayerMask targetMask, LayerMask obstacleMask)
    {
        Vector2 originPos = origin.position;
        Vector2 center = circle.bounds.center;
        float radius = circle.radius * Mathf.Max(circle.transform.lossyScale.x, circle.transform.lossyScale.y);

        Vector2 dirToCenter = (center - originPos).normalized;
        float distToCenter = Vector2.Distance(originPos, center);

        // 원 반지름으로 인한 시야각 확장
        float radiusAngle = 0f;
        if (distToCenter > 0f)
            radiusAngle = Mathf.Asin(Mathf.Clamp(radius / distToCenter, -1f, 1f)) * Mathf.Rad2Deg;

        float angleToCenter = Vector2.Angle(origin.up, dirToCenter);

        // 시야각 + 거리 체크
        if (angleToCenter - radiusAngle <= viewAngle / 2f && distToCenter - radius <= viewDistance)
        {
            if (!Physics2D.Raycast(originPos, dirToCenter, distToCenter, obstacleMask))
                return true;
        }

        return false;
    }
}
