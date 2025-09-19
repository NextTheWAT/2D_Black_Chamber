using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyStageNPC : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private LobbyStageDialogUI dialogUI;

    [Header("Prompt (optional")]
    [SerializeField] private GameObject pressFPrompt;

    private bool playerInRange = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (pressFPrompt) pressFPrompt.SetActive(true);
    }

    private void Update()
    {
        if (!playerInRange) return;
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            dialogUI?.Open();
            if (pressFPrompt) pressFPrompt.SetActive(false);
        }
    }
}
