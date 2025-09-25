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
        // 플레이어가 피해를 입었을 때 실행할 코드
        Debug.Log($"Player damaged! Current HP: {currentHP}/{maxHP}");
        if (animController == null) return;

        animController.PlayHit();
    }

    public void OnPlayerDie()
    {
        // 플레이어가 사망했을 때 실행할 코드
        Debug.Log("Player has died!");
        if (animController == null) return;

        topDownMovement.enabled = false; // 이동 비활성화
        playerInputController.enabled = false; // 입력 비활성화
        animController.PlayDie();
        GameManager.Instance.TriggerGameOver();
    }



}
