using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthEventHandler : MonoBehaviour
{
    private Health _health;

    private TopDownMovement topDownMovement;
    private PlayerInputController playerInputController;
    private CharacterAnimationController animController;

    // 마지막 공격자(사망 원인) 저장
    private string _lastKillerId = "unknown";
    public void ReportAttacker(string killerId) => _lastKillerId = string.IsNullOrEmpty(killerId) ? "unknown" : killerId;

    private void OnEnable()
    {
        _health = GetComponent<Health>();
        if (_health != null)
        {
            _health.OnHealthChanged.AddListener(OnPlayerDamaged);
            _health.OnDie.AddListener(OnPlayerDie);
        }
    }
    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnHealthChanged.RemoveListener(OnPlayerDamaged);
            _health.OnDie.RemoveListener(OnPlayerDie);
        }
    }

    private void Awake()
    {
        topDownMovement = GetComponent<TopDownMovement>();
        playerInputController = GetComponent<PlayerInputController>();
        animController = GetComponent<CharacterAnimationController>();
    }

    public void OnPlayerDamaged(int currentHP, int maxHP)
    {
        // 플레이어가 피해를 입었을 때 실행할 코드
        Debug.Log($"Player damaged! Current HP: {currentHP}/{maxHP}");
        if (animController == null) return;

        animController.PlayHit();
    }

    public void OnPlayerDie()
    {
        // 애널리틱스 훅 (널가드)
        var hook = GetComponent<PlayerDeathHook>();
        if (hook != null)
        {
            hook.OnDie(_lastKillerId);
        }

        // 플레이어가 사망했을 때 실행할 코드
        Debug.Log("Player has died!");
        if (animController == null) return;

        topDownMovement.enabled = false; // 이동 비활성화
        playerInputController.enabled = false; // 입력 비활성화
        animController.PlayDie();
        GameManager.Instance.TriggerGameOver();
    }



}
