using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPEffectsHandler : MonoBehaviour
{
    //  이 스크립트가 붙은 Image 컴포넌트입니다. (HP가 채워지는 Fill Image)
    [Header("HP Bar References")]
    [SerializeField] private Image fullHpBarImage; // HP 게이지 Image 컴포넌트

    [Header("HP Hit Effects")]
    [SerializeField] private float shakeDuration = 0.15f; // 진동 시간
    [SerializeField] private float shakeMagnitude = 5f; // 진동 강도 (픽셀 단위)
    [SerializeField] private ParticleSystem hitSprayParticle; // 피격 시 붉은색 스프레이 파티클

    // HP Bar 이미지의 RectTransform (진동 대상)
    private RectTransform _hpBarRectTransform;
    private Health _playerHealth;
    private int _lastHealth;
    private Coroutine _shakeCoroutine;
    private void Awake()
    {
        if (fullHpBarImage == null)
        {
            fullHpBarImage = GetComponent<Image>();
        }

        if (fullHpBarImage)
        {
            // HP 바 이미지의 RectTransform을 가져와 진동에 사용
            _hpBarRectTransform = fullHpBarImage.GetComponent<RectTransform>();
        }

        // 플레이어 Health 컴포넌트를 찾습니다.
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            _playerHealth = player.GetComponent<Health>();
        }
    }

    private void OnEnable()
    {
        if (_playerHealth)
        {
            _playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
            _lastHealth = _playerHealth.CurrentHealth;
            OnHealthChanged(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (_playerHealth)
            _playerHealth.OnHealthChanged.RemoveListener(OnHealthChanged);

        // 비활성화 시 진동 코루틴 정지
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }
    }

    private void OnHealthChanged(int cur, int max)
    {
        float ratio = max > 0 ? (float)cur / max : 0f;
        // 1. HP 바 시각적 갱신 (Image의 fillAmount만 사용)
        if (fullHpBarImage) fullHpBarImage.fillAmount = Mathf.Clamp01(ratio);

        // 2. 피격 효과
        if (cur < _lastHealth) // HP가 감소했을 때 (피격)
        {
            // HP Bar 진동 시작
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine); // 이미 진동 중이면 멈추고 새로 시작
            }
            // 진동 대상을 HP 바 자체(_hpBarRectTransform)로 지정
            _shakeCoroutine = StartCoroutine(ShakeUI(_hpBarRectTransform));

            // 스프레이 파티클 재생
            if (hitSprayParticle)
            {
                hitSprayParticle.Play();
            }
        }
        _lastHealth = cur; // 현재 HP를 다음 프레임의 이전 HP로 저장
    }

    // 지정된 RectTransform(UI 요소)을 흔드는 코루틴
    private IEnumerator ShakeUI(RectTransform targetTransform)
    {
        if (targetTransform == null)
        {
            yield break;
        }

        Vector3 originalPos = targetTransform.anchoredPosition3D;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            // UI의 위치를 흔듬
            targetTransform.anchoredPosition3D = originalPos + new Vector3(x, y, 0);
            elapsed += Time.unscaledDeltaTime; // 게임이 멈춰도 작동하도록 Unscaled Time 사용
            yield return null;
        }

        // 원래 위치로 복귀
        targetTransform.anchoredPosition3D = originalPos;
        _shakeCoroutine = null;

    }

}