using UnityEngine;
using UnityEngine.UI;

public class LoadingRunDemo : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator playerAnimator;   // �÷��̾� �ִϸ�����
    [SerializeField] private Image staminaFillImage;    // Image Type=Filled, Horizontal, Left

    [Header("Animator Params")]
    [SerializeField] private string runBoolName = "Run"; // �÷��̾� �ִϸ����� Bool �̸�

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float drainPerSec = 25f;  // �� �� ���ҷ�(�ʴ�)
    [SerializeField] private float regenPerSec = 18f;  // ���� �� ȸ����(�ʴ�)
    [SerializeField] private bool useUnscaledTime = true; // �ε� ȭ���̸� unscaled ����

    private float stamina;
    private bool isRunning;     // ���� �ε� �ִϸ��̼��� '�ٴ�' ��������

    void Awake()
    {
        stamina = maxStamina;
        ApplyStaminaToUI();
        SetRun(false); // ������ �ȱ�
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
                // �ٴڳ��� �ڵ����� �ȱ� ���·� ��ȯ
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

    // ===== �ִϸ��̼� �̺�Ʈ�� ȣ���� �޼��� 2�� =====
    // Pressed Ŭ�� ù ������
    public void AE_RunOn()
    {
        SetRun(true);
    }

    // Idle/WALK Ŭ�� ù ������
    public void AE_RunOff()
    {
        SetRun(false);
    }

    // ===== ���� ó�� =====
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
