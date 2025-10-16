using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public partial class PlayerInputController : TopDownController
{
    private HashSet<Iinteraction> iinteractions = new();

    [Header("UI Prompt")]
    [SerializeField] private GameObject fKeyPrompt; // �÷��̾� �ڽ��� FŰ ������ (�⺻ ��Ȱ��ȭ)

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (health.IsDead) return; // ��� �� ����

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
