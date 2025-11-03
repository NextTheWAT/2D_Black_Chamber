using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public partial class PlayerInputController : TopDownController
{
    private HashSet<Iinteraction> iinteractions = new();

    [Header("UI Prompt")]
    [SerializeField] private GameObject fKeyPrompt; // 플레이어 자식의 F키 프리팹 (기본 비활성화)

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (health.IsDead) return; // 사망 시 무시

        // TimeScale이 0 (UI 열림 상태)일 때, 이 컨트롤러의 상호작용 입력 무시.
        // StageSelectDialogueUI 내부의 F키 처리는 TimeScale이 0일 때도 정상 작동
        if (Time.timeScale == 0f)
        {
            return;
        }

        // 정상 상호작용 로직
        var temp = new HashSet<Iinteraction>(iinteractions);
        foreach (var interaction in temp)
            interaction?.Interaction(transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interaction")) return;

        var interaction = collision.GetComponent<Iinteraction>();
        if (interaction != null && !iinteractions.Contains(interaction))
        {
            iinteractions.Add(interaction);
            fKeyPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interaction")) return;

        var interaction = collision.GetComponent<Iinteraction>();
        if (interaction != null && iinteractions.Contains(interaction))
            iinteractions.Remove(collision.GetComponent<Iinteraction>());

        if (iinteractions.Count == 0)
            fKeyPrompt.SetActive(false);
    }
}