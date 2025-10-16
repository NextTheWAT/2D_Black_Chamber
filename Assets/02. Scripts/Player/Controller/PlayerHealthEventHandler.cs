using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthEventHandler : MonoBehaviour
{
    private Health _health;

    private TopDownMovement topDownMovement;
    private PlayerInputController playerInputController;
    private CharacterAnimationController animController;

    // ������ ������(��� ����) ����
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
        // �÷��̾ ���ظ� �Ծ��� �� ������ �ڵ�
        Debug.Log($"Player damaged! Current HP: {currentHP}/{maxHP}");
        if (animController == null) return;

        animController.PlayHit();
    }

    public void OnPlayerDie()
    {
        // �ֳθ�ƽ�� �� (�ΰ���)
        var hook = GetComponent<PlayerDeathHook>();
        if (hook != null)
        {
            hook.OnDie(_lastKillerId);
        }

        // �÷��̾ ������� �� ������ �ڵ�
        Debug.Log("Player has died!");
        if (animController == null) return;

        topDownMovement.enabled = false; // �̵� ��Ȱ��ȭ
        playerInputController.enabled = false; // �Է� ��Ȱ��ȭ
        animController.PlayDie();
        GameManager.Instance.TriggerGameOver();
    }



}
