using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 현재 장착 무기의 GunData.hasLaser / firePointOffset을 사용해
/// 레이저를 그려주는 단일 스크립트.
/// - WeaponManager.CurrentWeapon.gunData 를 읽어서 ON/OFF + 시작 위치 결정
/// - 무기 교체(잠입/난전 포함) 시 자동 갱신
/// - 별도 Pivot 없이 Shooter 로컬 오프셋을 TransformPoint로 변환
/// </summary>
public class AutoGunLaser : MonoBehaviour
{
    public enum Axis { Forward, Up, Right }

    [Header("Appearance")]
    public Color laserColor = Color.red;      // RGB만 사용(알파는 Gradient가 담당)
    [Range(0f, 1f)] public float startAlpha = 1f;    // 왼쪽(시작) 알파
    [Range(0f, 1f)] public float endAlpha = 0.01f; // 오른쪽(끝) 알파

    [Header("Appearance")]
    public bool tintDot = true; // Dot 프리팹 색도 함께 빨강으로 틴트할지

    [Header("Physics / Space")]
    [Tooltip("탑다운 2D면 체크. 3D면 해제.")]
    public bool use2D = true;
    [Tooltip("전진 축(2D 스프라이트는 보통 Right가 '앞')")]
    public Axis forwardAxis = Axis.Right;

    [Tooltip("GunData.firePointOffset(x,y) → 월드 단위 변환 배율")]
    public float offsetScale = 1f;

    public float maxDistance = 30f;
    public LayerMask hitMask = ~0;

    [Tooltip("3D에서 얇은 콜라이더 안정 검출")]
    public bool useSphereCast = false;
    public float sphereRadius = 0.02f;

    [Header("Line")]
    public LineRenderer line;
    public float lineWidth = 0.02f;
    [Tooltip("히트 지점에 찍을 점(선택)")]
    public GameObject dotPrefab;
    Transform dot;

    // 캐시
    Shooter currentShooter;
    GunData currentData;

    void Awake()
    {
        if (!line)
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.material = new Material(Shader.Find("Sprites/Default"));
        }

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        // 더 이상 startColor/endColor로 색/알파 주지 말기
        // line.startColor = laserColor;  // 제거
        // line.endColor   = laserColor;  // 제거

        // 머티리얼 알파를 1로 맞춰 Gradient 알파가 그대로 반영되게
        if (line.material)
        {
            var rgb1 = new Color(laserColor.r, laserColor.g, laserColor.b, 1f);
            if (line.material.HasProperty("_Color")) line.material.color = rgb1;
            if (line.material.HasProperty("_BaseColor")) line.material.SetColor("_BaseColor", rgb1);
        }

        ApplyLaserGradient();

        if (dotPrefab && dot == null)
        {
            dot = Instantiate(dotPrefab).transform;
            dot.gameObject.SetActive(false);

            // 선택: 레이저 색으로 점도 틴트
            if (tintDot)
            {
                var sr = dot.GetComponentInChildren<SpriteRenderer>(true);
                if (sr) sr.color = new Color(laserColor.r, laserColor.g, laserColor.b, 1f);
                else
                {
                    var r = dot.GetComponentInChildren<Renderer>(true);
                    if (r && r.material)
                    {
                        if (r.material.HasProperty("_Color"))
                            r.material.color = new Color(laserColor.r, laserColor.g, laserColor.b, 1f);
                        if (r.material.HasProperty("_BaseColor"))
                            r.material.SetColor("_BaseColor", new Color(laserColor.r, laserColor.g, laserColor.b, 1f));
                    }
                }
            }
        }
    }



    void OnEnable()
    {
        var wm = WeaponManager.Instance;
        if (wm) wm.OnWeaponChanged.AddListener(OnWeaponChanged);

        RefreshBinding();
        ApplyToggleImmediate();
    }

    void OnDisable()
    {
        var wm = WeaponManager.Instance;
        if (wm) wm.OnWeaponChanged.RemoveListener(OnWeaponChanged);

        if (line) line.enabled = false;
        if (dot) dot.gameObject.SetActive(false);
        currentShooter = null;
        currentData = null;
    }
    void ApplyLaserGradient()
    {
        var g = new Gradient();
        var rgb = new Color(laserColor.r, laserColor.g, laserColor.b, 1f); // 알파 1

        g.SetKeys(
            new[] {
            new GradientColorKey(rgb, 0f),
            new GradientColorKey(rgb, 1f)
            },
            new[] {
            new GradientAlphaKey(startAlpha, 0f), // 1.0 = 100%
            new GradientAlphaKey(endAlpha,   1f)  // 0.01 = 1%
            }
        );
        line.colorGradient = g;
    }

    void OnWeaponChanged(Shooter _)
    {
        RefreshBinding();
        ApplyToggleImmediate();
    }

    void RefreshBinding()
    {
        var wm = WeaponManager.Instance;
        currentShooter = wm ? wm.CurrentWeapon : null;
        currentData = currentShooter ? currentShooter.gunData : null;
    }

    void ApplyToggleImmediate()
    {
        bool on = currentData != null && currentData.hasLaser;
        if (line) line.enabled = on;
        if (!on && dot) dot.gameObject.SetActive(false);
        if (on) RenderLaser(); // 즉시 1회 갱신
    }

    void Update()
    {
        if (!currentShooter || currentData == null || !currentData.hasLaser)
        {
            if (line && line.enabled) line.enabled = false;
            if (dot && dot.gameObject.activeSelf) dot.gameObject.SetActive(false);
            return;
        }

        if (!line.enabled) line.enabled = true;
        RenderLaser();
    }

    void RenderLaser()
    {
        // 1) 시작점/방향 계산
        Transform t = currentShooter.transform;

        // GunData.firePointOffset(x,y) 사용
        Vector2 of2 = currentData.prefabInfo.firePointOffset * offsetScale;
        Vector3 local = use2D
            ? new Vector3(of2.x, of2.y, 0f)   // 2D(Top-Down, XY 평면)
            : new Vector3(of2.x, 0f, of2.y);  // 3D(XZ 평면)

        Vector3 start = t.TransformPoint(local);
        Vector3 dir = GetForward(t);

        // 2) 레이캐스트
        Vector3 end = start + dir * maxDistance;
        bool hit = false;
        Vector3 hitPoint = end, hitNormal = -dir;

        if (!use2D)
        {
            var ray = new Ray(start, dir);
            if (useSphereCast)
            {
                if (Physics.SphereCast(ray, sphereRadius, out var h, maxDistance, hitMask, QueryTriggerInteraction.Ignore))
                { hit = true; hitPoint = h.point; hitNormal = h.normal; }
            }
            else
            {
                if (Physics.Raycast(ray, out var h, maxDistance, hitMask, QueryTriggerInteraction.Ignore))
                { hit = true; hitPoint = h.point; hitNormal = h.normal; }
            }
        }
        else
        {
            var h2D = Physics2D.Raycast(start, dir, maxDistance, hitMask);
            if (h2D.collider != null)
            { hit = true; hitPoint = h2D.point; hitNormal = h2D.normal; }
        }

        // 3) 라인 & 도트
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.SetPosition(0, start);
        line.SetPosition(1, hit ? hitPoint : end);

        if (!dot) return;
        if (hit)
        {
            dot.position = hitPoint + hitNormal * 0.002f; // z-fighting 방지
            dot.rotation = use2D ? Quaternion.identity : Quaternion.LookRotation(hitNormal);
            if (!dot.gameObject.activeSelf) dot.gameObject.SetActive(true);
        }
        else if (dot.gameObject.activeSelf)
        {
            dot.gameObject.SetActive(false);
        }
    }

    Vector3 GetForward(Transform t)
    {
        switch (forwardAxis)
        {
            case Axis.Up: return t.up;
            case Axis.Right: return t.right;
            default: return t.forward;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!line) return;
        // 머티리얼 알파 1 유지
        if (line.material)
        {
            var rgb1 = new Color(laserColor.r, laserColor.g, laserColor.b, 1f);
            if (line.material.HasProperty("_Color")) line.material.color = rgb1;
            if (line.material.HasProperty("_BaseColor")) line.material.SetColor("_BaseColor", rgb1);
        }
        ApplyLaserGradient();
    }
#endif
}
