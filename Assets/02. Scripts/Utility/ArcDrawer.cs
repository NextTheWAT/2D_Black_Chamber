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

    public Color GetColor()
        => lineRenderer.startColor;


    public void SetColor(Color color)
    {
        color.a = lineRenderer.startColor.a; // 기존 알파 값 유지

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void SetAlpha(float alpha)
    {
        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;

        startColor.a = alpha;
        endColor.a = alpha;

        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }

    public void DrawArc(float totalAngle)
    {
        if(totalAngle < 0)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;

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
