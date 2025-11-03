using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform player;   // 플레이어 Transform
    [SerializeField] private float followSpeed = 5f; // 카메라가 플레이어를 따라가는 속도

    [Header("Mouse Offset Settings")]
    [Min(0f), SerializeField] private float maxCameraDistance = 4f; // 오프셋 최대 거리
    private float currentCameraDistance = 0f;

    [Header("Shake Settings")]
    [SerializeField] private float shakeMagnitude = 0.2f;
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeMultiplier = 1f;
    private Vector3 shakeOffset;
    private Coroutine shakeCo;

    public Transform CamTr => Camera.main.transform;

    public float MaxCameraDistance => maxCameraDistance;
    public float CurrentCameraDistance
    {
        get => currentCameraDistance;
        set => currentCameraDistance = Mathf.Max(value, 0f);
    }

    private void Awake()
        => CurrentCameraDistance = maxCameraDistance;

    void LateUpdate()
    {
        if (player == null) return;

        // 마우스 위치 → 월드 좌표 변환
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 플레이어 → 마우스 방향 벡터 계산
        Vector2 dir = (mouseWorld - (Vector2)player.position).normalized;

        // 마우스와 플레이어 사이 거리 계산
        float distance = Vector2.Distance(mouseWorld, player.position);

        // 오프셋 거리 제한 (최대 거리 제한)
        float clampedDistance = Mathf.Min(distance, currentCameraDistance);

        // 최종 카메라 목표 위치 계산
        Vector3 targetPosition = player.position + (Vector3)(dir * clampedDistance);
        targetPosition.z = -10f;

        // 거리에 비례해서 카메라 이동속도 변경
        float dynamicFollowSpeed = followSpeed * (clampedDistance / maxCameraDistance);

        // 부드럽게 따라가기
        CamTr.position = Vector3.Lerp(CamTr.position, targetPosition, dynamicFollowSpeed * Time.deltaTime) + shakeOffset;
    }


    // 카메라 흔들림
    public void ShakeCamera(float magnitude = -1f, float duration = -1f)
    {
        if (shakeCo != null)
            StopCoroutine(shakeCo);

        duration = duration > 0 ? duration : shakeDuration;
        magnitude = magnitude > 0 ? magnitude : shakeMagnitude;

        shakeCo = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            shakeOffset.x = Random.Range(-magnitude, magnitude);
            shakeOffset.y = Random.Range(-magnitude, magnitude);
            shakeOffset *= shakeMultiplier;

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeCo = null;
    }
}
