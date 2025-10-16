using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using Constants;

public class CCTV : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private float angularSpeed = 120f;
    [SerializeField] private float rotationDelay = 3f;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private float viewAngle = 45f;
    [SerializeField] private float rotateRange = 270f; // 회전 범위 (좌우 합)
    [SerializeField] private float suspicionBuildTime = 4f;

    [Header("Detection")]
    [SerializeField] private Transform head;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private Light2D light2D;

    [SerializeField] private Color originalColor;
    [SerializeField] private Color suspiciousColor;
    [SerializeField] private Color alertColor;

    private Transform detectedTarget;
    private bool isLeftRotating;

    private float centerZ;
    private float leftLimitZ;
    private float rightLimitZ;

    public float EulerZ => head.eulerAngles.z;

    private void Start()
    {
        light2D.pointLightOuterRadius = viewDistance;
        light2D.pointLightOuterAngle = viewAngle;

        originalColor = light2D.color;

        // 시작 방향을 중심으로 회전 각도 계산
        centerZ = head.eulerAngles.z;
        float halfRange = rotateRange * 0.5f;
        leftLimitZ = NormalizeAngle(centerZ + halfRange);
        rightLimitZ = NormalizeAngle(centerZ - halfRange);

        isLeftRotating = true;
        SetEulerZ(rightLimitZ);

        StartCoroutine(Watch());
    }

    private void OnEnable()
        => GameManager.Instance.OnPhaseChanged += OnPhaseChanged;

    private void OnDisable(){
        if (GameManager.AppIsQuitting) return;
        GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    void OnPhaseChanged(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.Combat)
            light2D.color = alertColor;
    }

    void DetectTarget()
        => detectedTarget = head.FindTargetInFOV(viewAngle, viewDistance, targetMask, obstacleMask);

    void SetEulerZ(float eulerZ)
        => head.eulerAngles = new Vector3(0, 0, eulerZ);

    // 감시
    IEnumerator Watch()
    {
        float elapsedTime = 0f;

        light2D.color = GameManager.Instance.IsCombat ? alertColor : originalColor;

        while (!detectedTarget)
        {
            float deltaZ = angularSpeed * Time.deltaTime;
            float currentZ = EulerZ;

            if (isLeftRotating)
            {
                currentZ += deltaZ;
                if (AnglePassed(currentZ, leftLimitZ, centerZ))
                {
                    elapsedTime = 0f;
                    currentZ = leftLimitZ;
                    isLeftRotating = false;

                    while (elapsedTime < rotationDelay)
                    {
                        DetectTarget();
                        if (detectedTarget) break;

                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                }
            }
            else
            {
                currentZ -= deltaZ;
                if (AnglePassed(currentZ, rightLimitZ, centerZ, false))
                {
                    elapsedTime = 0f;
                    currentZ = rightLimitZ;
                    isLeftRotating = true;

                    while (elapsedTime < rotationDelay)
                    {
                        DetectTarget();
                        if (detectedTarget) break;

                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                }
            }

            SetEulerZ(currentZ);
            DetectTarget();
            yield return null;
        }

        StartCoroutine(Suspect());
    }

    IEnumerator Suspect()
    {
        light2D.color = GameManager.Instance.IsCombat ? alertColor : suspiciousColor;

        float elapsedTime = 0f;

        while (detectedTarget)
        {
            elapsedTime += Time.deltaTime;

            Vector2 dir = (detectedTarget.position - head.position).normalized;
            float targetZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.MoveTowardsAngle(EulerZ, targetZ, angularSpeed * Time.deltaTime);

            // leftLimitZ, rightLimitZ 범위 내로 제한
            angle = ClampAngleToLimits(angle, centerZ);

            SetEulerZ(angle);
            DetectTarget();

            if (elapsedTime >= suspicionBuildTime)
            {
                GameManager.Instance.IsCombat = true;
                break;
            }

            yield return null;
        }

        StartCoroutine(Watch());
    }
    // leftLimitZ, rightLimitZ 사이로 제한하는 함수
    float ClampAngleToLimits(float current, float center)
    {
        float halfRange = rotateRange * 0.5f;

        // 중심 기준으로 현재 각도의 상대적 차이 (음수~양수)
        float diff = Mathf.DeltaAngle(center, current);

        // 제한 범위를 벗어나면 clamp
        diff = Mathf.Clamp(diff, -halfRange, halfRange);

        // 다시 실제 회전 각도로 변환
        return NormalizeAngle(center + diff);
    }

    // 각도 정규화
    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }

    // 좌우 회전 시 각도 범위 체크
    bool AnglePassed(float current, float limit, float center, bool isLeft = true)
    {
        float diff = Mathf.DeltaAngle(center, current);
        float limitDiff = Mathf.DeltaAngle(center, limit);
        return isLeft ? diff >= limitDiff : diff <= limitDiff;
    }

    public void Die()
    {
        StructSoundManager.Instance.PlayStructBrokenSound();
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        // viewDistance 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(head.position, viewDistance);

        // viewAngle 표시
        Vector2 rightDir = Quaternion.Euler(0, 0, viewAngle * 0.5f) * head.up;
        Vector2 leftDir = Quaternion.Euler(0, 0, -viewAngle * 0.5f) * head.up;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(head.position, (Vector2)head.position + rightDir * viewDistance);
        Gizmos.DrawLine(head.position, (Vector2)head.position + leftDir * viewDistance);

        // 회전 범위 표시
        float centerZ = Application.isPlaying ? this.centerZ : head.eulerAngles.z;
        float halfRange = rotateRange * 0.5f;

        Vector2 leftLimitDir = Quaternion.Euler(0, 0, centerZ + halfRange) * Vector2.up;
        Vector2 rightLimitDir = Quaternion.Euler(0, 0, centerZ - halfRange) * Vector2.up;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(head.position, (Vector2)head.position + leftLimitDir * viewDistance);
        Gizmos.DrawLine(head.position, (Vector2)head.position + rightLimitDir * viewDistance);
    }
}
