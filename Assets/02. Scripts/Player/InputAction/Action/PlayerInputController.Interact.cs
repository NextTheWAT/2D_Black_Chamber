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
