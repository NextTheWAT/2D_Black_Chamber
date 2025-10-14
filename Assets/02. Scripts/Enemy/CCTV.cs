using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class CCTV : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private float angularSpeed = 120f;
    [SerializeField] private float rotationDelay = 3f;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private float viewAngle = 45f;
    [SerializeField] private float rotateRange = 270f; // ȸ�� ���� (�¿� ��)
    [SerializeField] private float suspicionBuildTime = 4f;

    [Header("Detection")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private Light2D light2D;

    [SerializeField] private Color originalColor;
    [SerializeField] private Color suspiciousColor;
    [SerializeField] private Color alertColor;

    private Transform detectedTarget;
    private bool isLeftRotation;

    private float centerZ;
    private float leftLimitZ;
    private float rightLimitZ;

    public float EulerZ => transform.eulerAngles.z;

    private void Start()
    {
        light2D.pointLightOuterRadius = viewDistance;
        light2D.pointLightOuterAngle = viewAngle;

        originalColor = light2D.color;

        // ���� ������ �߽����� ȸ�� ���� ���
        centerZ = transform.eulerAngles.z;
        float halfRange = rotateRange * 0.5f;
        leftLimitZ = NormalizeAngle(centerZ + halfRange);
        rightLimitZ = NormalizeAngle(centerZ - halfRange);

        isLeftRotation = true;
        SetEulerZ(rightLimitZ);

        StartCoroutine(Watch());
    }

    private void Update()
    {
        DetectTarget();
    }

    void DetectTarget()
        => detectedTarget = transform.FindTargetInFOV(viewAngle, viewDistance, targetMask, obstacleMask);

    void SetEulerZ(float eulerZ)
        => transform.eulerAngles = new Vector3(0, 0, eulerZ);

    // ����
    IEnumerator Watch()
    {
        light2D.color = GameManager.Instance.IsCombat ? alertColor : originalColor;

        while (!detectedTarget)
        {
            float deltaZ = angularSpeed * Time.deltaTime;
            float currentZ = EulerZ;

            if (isLeftRotation)
            {
                currentZ += deltaZ;
                if (AnglePassed(currentZ, leftLimitZ, centerZ))
                {
                    currentZ = leftLimitZ;
                    isLeftRotation = false;
                    SetEulerZ(currentZ);
                    yield return new WaitForSeconds(rotationDelay);
                }
            }
            else
            {
                currentZ -= deltaZ;
                if (AnglePassed(currentZ, rightLimitZ, centerZ, false))
                {
                    currentZ = rightLimitZ;
                    isLeftRotation = true;
                    SetEulerZ(currentZ);
                    yield return new WaitForSeconds(rotationDelay);
                }
            }

            SetEulerZ(currentZ);
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

            Vector2 dir = (detectedTarget.position - transform.position).normalized;
            float targetZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.MoveTowardsAngle(EulerZ, targetZ, angularSpeed * Time.deltaTime);

            // leftLimitZ, rightLimitZ ���� ���� ����
            angle = ClampAngleToLimits(angle, centerZ);

            SetEulerZ(angle);

            if (elapsedTime >= suspicionBuildTime)
            {
                GameManager.Instance.IsCombat = true;
                break;
            }

            yield return null;
        }

        StartCoroutine(Watch());
    }
    // leftLimitZ, rightLimitZ ���̷� �����ϴ� �Լ�
    float ClampAngleToLimits(float current, float center)
    {
        float halfRange = rotateRange * 0.5f;

        // �߽� �������� ���� ������ ����� ���� (����~���)
        float diff = Mathf.DeltaAngle(center, current);

        // ���� ������ ����� clamp
        diff = Mathf.Clamp(diff, -halfRange, halfRange);

        // �ٽ� ���� ȸ�� ������ ��ȯ
        return NormalizeAngle(center + diff);
    }

    // ���� ����ȭ
    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }

    // �¿� ȸ�� �� ���� ���� üũ
    bool AnglePassed(float current, float limit, float center, bool isLeft = true)
    {
        float diff = Mathf.DeltaAngle(center, current);
        float limitDiff = Mathf.DeltaAngle(center, limit);
        return isLeft ? diff >= limitDiff : diff <= limitDiff;
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        // viewDistance ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        // viewAngle ǥ��
        Vector2 rightDir = Quaternion.Euler(0, 0, viewAngle * 0.5f) * transform.up;
        Vector2 leftDir = Quaternion.Euler(0, 0, -viewAngle * 0.5f) * transform.up;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightDir * viewDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftDir * viewDistance);

        // ȸ�� ���� ǥ��
        float centerZ = Application.isPlaying ? this.centerZ : transform.eulerAngles.z;
        float halfRange = rotateRange * 0.5f;

        Vector2 leftLimitDir = Quaternion.Euler(0, 0, centerZ + halfRange) * Vector2.up;
        Vector2 rightLimitDir = Quaternion.Euler(0, 0, centerZ - halfRange) * Vector2.up;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftLimitDir * viewDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightLimitDir * viewDistance);
    }
}
