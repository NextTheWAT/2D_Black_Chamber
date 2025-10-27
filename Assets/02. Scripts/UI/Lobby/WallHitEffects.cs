using UnityEngine;
using System.Collections;

public class WallHitEffects : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.15f;  // 흔들림 유지 시간
    [SerializeField] private float shakeMagnitude = 0.1f;  // 흔들림 강도 (월드 좌표 기준)
    [SerializeField] private float shakeFrequency = 30f;   // 초당 진동 빈도

    // 원본 위치 및 코루틴 관리 변수
    private Vector3 _originalPosition;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        // 오브젝트의 초기 위치를 저장합니다.
        _originalPosition = transform.position;
    }

    // 외부(Bullet 스크립트)에서 피격 이벤트를 받을 때 호출됩니다.
    public void ReceiveHit()
    {
        // 이미 흔들림 중이라면 기존 코루틴을 멈추고 새로 시작합니다.
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            transform.position = _originalPosition; // 위치 복귀
        }

        _shakeCoroutine = StartCoroutine(ShakeObject());
    }

    /// 오브젝트를 흔드는 코루틴입니다.
    private IEnumerator ShakeObject()
    {
        float elapsed = 0f;
        float delay = 1f / shakeFrequency; // 진동 간격

        while (elapsed < shakeDuration)
        {
            // 원본 위치를 기준으로 무작위로 위치를 이동시킵니다.
            // Random.insideUnitCircle을 사용하여 2D 평면에서 원형으로 흔들림.
            Vector2 randomShake = Random.insideUnitCircle * shakeMagnitude;

            // 2D 게임이므로 Z축은 유지하고 XY만 흔듭니다.
            transform.position = _originalPosition + (Vector3)randomShake;

            elapsed += delay;
            yield return new WaitForSeconds(delay); // 지정된 빈도에 따라 대기
        }

        // 흔들림이 끝나면 원래 위치로 복귀
        transform.position = _originalPosition;
        _shakeCoroutine = null;
    }

    private void OnDisable()
    {
        // 오브젝트 비활성화 시 코루틴 정지 및 위치 복귀
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }
    }
}
