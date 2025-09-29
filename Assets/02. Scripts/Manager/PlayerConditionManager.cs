using UnityEngine;
using System;

[DisallowMultipleComponent]
public class PlayerConditionManager : Singleton<PlayerConditionManager>
{
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float runDrainPerSec = 20f;      // �޸� �� �ʴ� �Ҹ�
    [SerializeField] private float regenPerSec = 12f;         // ��� �ӵ�
    [SerializeField] private float regenDelayAfterRun = 2f;   // �޸��� ���� �� ��� ����
    [SerializeField] private float minStaminaToRun = 5f;      // �� �� �̸��̸� �޸��� �Ұ�

    private float stamina;
    private float regenUnlockTime;
    public event Action<float> OnStamina01Changed; // UI ���ε���(0~1)

    public float Stamina01 => Mathf.InverseLerp(0f, maxStamina, stamina);
    public bool CanRun => stamina > 0f;

    private void Awake()
    {
        base.Awake();
        stamina = maxStamina;
        Notify();
    }

    public void ConsumeForRun(float dt)
    {
        stamina -= runDrainPerSec * dt;
        stamina = Mathf.Max(0f, stamina);
        regenUnlockTime = Time.time + regenDelayAfterRun;
        Notify();

        Debug.Log($"[Stamina] �޸��� �� {stamina}/{maxStamina}");
    }

    public void TickRegen(float dt)
    {
        if (Time.time < regenUnlockTime) return;
        if (stamina >= maxStamina) return;

        stamina += regenPerSec * dt;
        stamina = Mathf.Min(maxStamina, stamina);
        Notify();

        Debug.Log($"[Stamina] ȸ�� �� {stamina}/{maxStamina}");
    }

    private void Notify() => OnStamina01Changed?.Invoke(Stamina01);

    // �ʿ� �� �ܺο��� �� ����/�б� �޼��� �߰� ����
}
