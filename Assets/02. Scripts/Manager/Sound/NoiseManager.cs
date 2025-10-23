using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NoiseManager : Singleton<NoiseManager>
{
    [Header("Noise Settings")]
    [SerializeField, Range(0f, 1f)] private float obstacleNoiseReductionRate = 0.3f;
    [SerializeField] private float distanceFalloff = 1f;
    [SerializeField] private float noiseRange = 5f;

    [Header("Noise Data")]
    [SerializeField] private float walkNoise = 22f;
    [SerializeField] private float runNoise = 25f;
    [SerializeField] private float shootNoise = 60f;
    public float WalkNoise => walkNoise;
    public float RunNoise => runNoise;
    public float ShootNoise => shootNoise;

    [Header("Noise Threshold")]
    [SerializeField] private float combatThreshold = 30f;
    [SerializeField] private float investigateThreshold = 20f;
    public float CombatThreshold => combatThreshold;
    public float InvestigateThreshold => investigateThreshold;

    [SerializeField] private float gizmoDuration = 1f; // 기즈모 유지 시간

    private struct NoiseSegment
    {
        public Vector3 start;
        public Vector3 end;
        public float strength;
    }

    private struct NoiseData
    {
        public Vector2 position;
        public float time;
        public List<NoiseSegment> segments;
    }

    private readonly List<NoiseData> noiseHistory = new();

    public void EmitNoise(Transform sender, Vector2 position, float baseNoise)
    {
        if (GameManager.Instance.IsCombat) return;
        if (noiseRange <= 0) return;
        if (sender && (sender.gameObject.layer | GameManager.Instance.enemyLayerMask) == GameManager.Instance.enemyLayerMask) return;

        var segments = new List<NoiseSegment>();
        float radius = noiseRange * 0.5f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius, GameManager.Instance.enemyLayerMask);

        foreach (var coll in colliders)
        {
            Enemy enemy = coll.GetComponent<Enemy>();
            if (enemy == null) continue;

            // 직선 경로 구간 소음 계산
            var lineSegments = CalculateRaySegments(baseNoise, position, enemy.transform.position);
            segments.AddRange(lineSegments);

            // NavMesh 경로 구간 소음 계산
            var navSegments = CalculateNavSegments(baseNoise, position, enemy.transform.position);
            segments.AddRange(navSegments);

            // 적에게 최대 소음 알려주기
            float directNoise = lineSegments.Count > 0 ? lineSegments[^1].strength : 0f;
            float navNoise = navSegments.Count > 0 ? navSegments[^1].strength : 0f;
            float finalNoise = Mathf.Max(directNoise, navNoise);

            enemy.HeardNoise(finalNoise, position);
        }

        noiseHistory.Add(new NoiseData
        {
            position = position,
            time = Time.time,
            segments = segments
        });
    }

    // 직선 경로 구간별 소음 계산
    private List<NoiseSegment> CalculateRaySegments(float baseNoise, Vector2 start, Vector2 end)
    {
        var segments = new List<NoiseSegment>();
        RaycastHit2D[] hits = Physics2D.LinecastAll(start, end, GameManager.Instance.obstacleLayerMask);

        Vector2 segmentStart = start;
        float remainingNoise = baseNoise;

        foreach (var hit in hits)
        {
            Vector2 segmentEnd = hit.point;
            float distance = Vector2.Distance(segmentStart, segmentEnd);
            float segmentNoise = Mathf.Max(0f, remainingNoise - distance * distanceFalloff);

            segments.Add(new NoiseSegment
            {
                start = segmentStart,
                end = segmentEnd,
                strength = segmentNoise,
            });

            remainingNoise = segmentNoise * (1f - obstacleNoiseReductionRate);
            segmentStart = hit.point;
        }

        if (Vector2.Distance(segmentStart, end) > 0.01f)
        {
            float distance = Vector2.Distance(segmentStart, end);
            float segmentNoise = Mathf.Max(0f, remainingNoise - distance * distanceFalloff);

            segments.Add(new NoiseSegment
            {
                start = segmentStart,
                end = end,
                strength = segmentNoise,
            });
        }

        return segments;
    }

    // NavMesh 경로 구간별 소음 계산
    private List<NoiseSegment> CalculateNavSegments(float baseNoise, Vector2 start, Vector2 end)
    {
        var segments = new List<NoiseSegment>();
        Vector3[] corners = GetNavPathCorners(start, end);
        if (corners == null || corners.Length < 2) return segments;

        float remainingNoise = baseNoise;
        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector2 segmentStart = corners[i];
            Vector2 segmentEnd = corners[i + 1];
            float distance = Vector2.Distance(segmentStart, segmentEnd);
            RaycastHit2D[] hits = Physics2D.LinecastAll(segmentStart, segmentEnd, GameManager.Instance.obstacleLayerMask);
            int hitCount = hits.Length;

            float segmentNoise = Mathf.Max(0f, remainingNoise - distance * distanceFalloff);
            segments.Add(new NoiseSegment
            {
                start = segmentStart,
                end = segmentEnd,
                strength = segmentNoise
            });

            remainingNoise = segmentNoise * Mathf.Pow(1f - obstacleNoiseReductionRate, hitCount);
        }

        return segments;
    }

    private Vector3[] GetNavPathCorners(Vector2 start, Vector2 end2D)
    {
        NavMeshPath path = new();
        if (NavMesh.CalculatePath(start, end2D, NavMesh.AllAreas, path))
            return path.corners;
        return null;
    }

    private void OnDrawGizmos()
    {
        if (noiseHistory == null) return;

        for (int i = noiseHistory.Count - 1; i >= 0; i--)
        {
            var data = noiseHistory[i];
            float elapsed = Time.time - data.time;

            if (elapsed > gizmoDuration)
            {
                noiseHistory.RemoveAt(i);
                continue;
            }

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(data.position, noiseRange * 0.5f);

            foreach (var seg in data.segments)
            {
                Color color = seg.strength >= combatThreshold ? Color.red :
                              seg.strength >= investigateThreshold ? Color.yellow :
                              Color.white;
                Gizmos.color = color;
                Gizmos.DrawLine(seg.start, seg.end);

#if UNITY_EDITOR
                Handles.Label((seg.start + seg.end) * 0.5f, seg.strength.ToString("F1"));
#endif
            }
        }
    }
}
