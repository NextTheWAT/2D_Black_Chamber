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
    [SerializeField] private float combatThreshold = 30f; // 전투 모드 진입 임계값
    [SerializeField] private float investigateThreshold = 20f; // 조사 모드 진입 임계값

    public float CombatThreshold => combatThreshold;
    public float InvestigateThreshold => investigateThreshold;


    private struct NoisePath
    {
        public Vector3[] navPath;
        public Vector3 start;
        public Vector3 end;
        public float strength;
        public bool isNavPath;
    }

    private struct NoiseData
    {
        public Vector2 position;
        public float time;
        public List<NoisePath> paths;
    }

    private readonly List<NoiseData> noiseHistory = new();
    [SerializeField] private float gizmoDuration = 1f; // 기즈모 유지 시간

    public void EmitNoise(Transform sender, Vector2 position, float baseNoise)
    {
        if (GameManager.Instance.IsCombat) return;
        if (noiseRange <= 0) return;
        if(sender && (sender.gameObject.layer | GameManager.Instance.enemyLayerMask) == GameManager.Instance.enemyLayerMask) return;

        var paths = new List<NoisePath>();
        float radius = noiseRange * 0.5f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius, GameManager.Instance.enemyLayerMask);

        foreach (var coll in colliders)
        {
            Enemy enemy = coll.GetComponent<Enemy>();
            if (enemy == null) continue;

            // 직선 경로 소음
            float rayNoise = GetRayPathNoise(baseNoise, enemy, position, out RaycastHit2D[] rayHits);
            paths.Add(new NoisePath
            {
                start = enemy.transform.position,
                end = position,
                strength = rayNoise,
                isNavPath = false
            });

            // NavMesh 경로 소음
            float navNoise = GetAgentPathNoise(baseNoise, enemy, position, out var navCorners);
            if (navCorners != null)
            {
                paths.Add(new NoisePath
                {
                    navPath = navCorners,
                    strength = navNoise,
                    isNavPath = true
                });

                Debug.Log(navCorners.Length);
            }

            float finalNoise = Mathf.Max(rayNoise, navNoise);
            enemy.HeardNoise(finalNoise, position);
        }

        noiseHistory.Add(new NoiseData
        {
            position = position,
            time = Time.time,
            paths = paths
        });
    }

    private float GetRayPathNoise(float baseNoise, Enemy enemy, Vector2 noisePosition, out RaycastHit2D[] raycastHits)
    {
        float distance = Vector2.Distance(enemy.transform.position, noisePosition);
        raycastHits = Physics2D.LinecastAll(enemy.transform.position, noisePosition, GameManager.Instance.obstacleLayerMask);

        int hitCount = raycastHits.Length;
        float noise = baseNoise - distance * distanceFalloff;
        noise *= Mathf.Pow(1f - obstacleNoiseReductionRate, hitCount);
        return Mathf.Max(0f, noise);
    }

    private float GetAgentPathNoise(float baseNoise, Enemy enemy, Vector2 noisePosition, out Vector3[] corners)
    {
        corners = GetPathCorners(enemy.transform.position, noisePosition);
        if (corners == null || corners.Length < 2) return 0f;

        float totalDist = 0f;
        int hitCount = 0;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            totalDist += Vector2.Distance(corners[i], corners[i + 1]);
            RaycastHit2D[] hits = Physics2D.LinecastAll(corners[i], corners[i + 1], GameManager.Instance.obstacleLayerMask);
            hitCount += hits.Length;
        }

        float noise = baseNoise - totalDist * distanceFalloff;
        noise *= Mathf.Pow(1f - obstacleNoiseReductionRate, hitCount);
        return Mathf.Max(0f, noise);
    }

    private Vector3[] GetPathCorners(Vector2 start, Vector2 end)
    {
        NavMeshPath path = new();
        if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path))
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

            // 중심 원
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(data.position, noiseRange * 0.5f);

            // 각 경로별 표시
            foreach (var path in data.paths)
            {

                if (path.isNavPath && path.navPath != null)
                {
                    for (int j = 0; j < path.navPath.Length - 1; j++)
                    {
                        Color color = path.strength >= combatThreshold ? Color.red : path.strength >= investigateThreshold ? Color.yellow : Color.white;
                        Gizmos.color = color;
                        Gizmos.DrawLine(path.navPath[j], path.navPath[j + 1]);
                        #if UNITY_EDITOR
                        Handles.Label((path.navPath[j] + path.navPath[j + 1]) * 0.5f, path.strength.ToString("F1"));
                        #endif
                    }
                }
                else
                {
                    Color color = path.strength >= combatThreshold ? Color.red : path.strength >= investigateThreshold ? Color.yellow : Color.white;
                    Gizmos.color = color;
                    Gizmos.DrawLine(path.start, path.end);
                    #if UNITY_EDITOR
                    Handles.Label((path.start + path.end) * 0.5f, path.strength.ToString("F1"));
                    #endif
                }
            }


        }
    }
}
