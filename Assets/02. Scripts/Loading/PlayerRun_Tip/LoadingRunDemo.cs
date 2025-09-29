using UnityEngine;
using UnityEngine.UI;

public class LoadingRunDemo : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator playerAnimator;   // 플레이어 애니메이터
    [SerializeField] private Image staminaFillImage;    // Image Type=Filled, Horizontal, Left

    [Header("Animator Params")]
    [SerializeField] private string runBoolName = "Run"; // 플레이어 애니메이터 Bool 이름

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float drainPerSec = 25f;  // 뛸 때 감소량(초당)
    [SerializeField] private float regenPerSec = 18f;  // 걸을 때 회복량(초당)
    [SerializeField] private bool useUnscaledTime = true; // 로딩 화면이면 unscaled 권장

    private float stamina;
    private bool isRunning;     // 현재 로딩 애니메이션이 '뛰는' 상태인지

    void Awake()
    {
        stamina = maxStamina;
        ApplyStaminaToUI();
        SetRun(false); // 시작은 걷기
    }

    void Update()
    {
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (isRunning)
        {
            stamina -= drainPerSec * dt;
            if (stamina <= 0f)
            {
                stamina = 0f;
                // 바닥나면 자동으로 걷기 상태로 전환
                SetRun(false);
            }
        }
        else
        {
            stamina += regenPerSec * dt;
            if (stamina > maxStamina) stamina = maxStamina;
        }

        ApplyStaminaToUI();
    }

    // ===== 애니메이션 이벤트로 호출할 메서드 2개 =====
    // Pressed 클립 첫 프레임
    public void AE_RunOn()
    {
        SetRun(true);
    }

    // Idle/WALK 클립 첫 프레임
    public void AE_RunOff()
    {
        SetRun(false);
    }

    // ===== 내부 처리 =====
    private void SetRun(bool run)
    {
        isRunning = run;
        if (playerAnimator && !string.IsNullOrEmpty(runBoolName))
            playerAnimator.SetBool(runBoolName, run);
    }

    private void ApplyStaminaToUI()
    {
        if (staminaFillImage)
            staminaFillImage.fillAmount = Mathf.InverseLerp(0f, maxStamina, stamina);
    }
}
