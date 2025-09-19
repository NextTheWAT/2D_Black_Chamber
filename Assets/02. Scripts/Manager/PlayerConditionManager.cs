using UnityEngine;
using System;

[DisallowMultipleComponent]
public class PlayerConditionManager : Singleton<PlayerConditionManager>
{
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float runDrainPerSec = 20f;      // 달릴 때 초당 소모
    [SerializeField] private float regenPerSec = 12f;         // 재생 속도
    [SerializeField] private float regenDelayAfterRun = 0.75f;// 달리기 멈춘 뒤 재생 지연
    [SerializeField] private float minStaminaToRun = 5f;      // 이 값 미만이면 달리기 불가

    private float stamina;
    private float regenUnlockTime;
    public event Action<float> OnStamina01Changed; // UI 바인딩용(0~1)

    public float Stamina01 => Mathf.InverseLerp(0f, maxStamina, stamina);
    public bool CanRun => stamina > minStaminaToRun;

    private void Awake()
    {
        stamina = maxStamina;
        Notify();
    }

    public void ConsumeForRun(float dt)
    {
        stamina -= runDrainPerSec * dt;
        stamina = Mathf.Max(0f, stamina);
        regenUnlockTime = Time.time + regenDelayAfterRun;
        Notify();
    }

    public void TickRegen(float dt)
    {
        if (Time.time < regenUnlockTime) return;
        if (stamina >= maxStamina) return;

        stamina += regenPerSec * dt;
        stamina = Mathf.Min(maxStamina, stamina);
        Notify();
    }

    private void Notify() => OnStamina01Changed?.Invoke(Stamina01);

    // 필요 시 외부에서 값 세팅/읽기 메서드 추가 가능
}
