using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthEventHandler : MonoBehaviour
{
    private TopDownMovement topDownMovement;
    private PlayerInputController playerInputController;
    private CharacterAnimationController animController;

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
        // �÷��̾ ������� �� ������ �ڵ�
        Debug.Log("Player has died!");
        if (animController == null) return;

        topDownMovement.enabled = false; // �̵� ��Ȱ��ȭ
        playerInputController.enabled = false; // �Է� ��Ȱ��ȭ
        animController.PlayDie();
        GameManager.Instance.TriggerGameOver();
    }



}
