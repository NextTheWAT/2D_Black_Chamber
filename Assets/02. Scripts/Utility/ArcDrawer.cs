using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ArcDrawer : MonoBehaviour
{
    public float radius = 5f;            // 반지름
    public int segmentCount = 30;        // 세분화 정도

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;
    }

    public void DrawArc(float totalAngle)
    {
        float angleStep = totalAngle / (segmentCount - 1);
        float halfAngle = totalAngle * 0.5f;

        for (int i = 0; i < segmentCount; i++)
        {
            float currentAngle = -halfAngle + (angleStep * i);
            float rad = (currentAngle + 90f) * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            lineRenderer.SetPosition(i, pos);
        }
    }
}
