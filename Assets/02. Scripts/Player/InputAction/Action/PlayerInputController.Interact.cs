using UnityEngine.InputSystem;
using UnityEngine;

public partial class PlayerInputController : TopDownController
{
    private Iinteraction _current; // 지금 상호작용 가능한 대상

    [Header("UI Prompt")]
    [SerializeField] private GameObject fKeyPrompt; // 플레이어 자식의 F키 프리팹 (기본 비활성화)

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        _current?.Interaction(transform);
        // 상호작용 직후에 프롬프트를 유지할지/숨길지는 취향에 따라:
        // if (_current == null) fKeyPrompt?.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interaction"))
        {
            _current = other.GetComponent<Iinteraction>();
            if (_current != null) fKeyPrompt?.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Interaction")) return;

        if (_current != null && _current == other.GetComponent<Iinteraction>())
        {
            _current = null;
            fKeyPrompt?.SetActive(false);
        }
    }
}
