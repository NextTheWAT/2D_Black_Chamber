using UnityEngine.InputSystem;
using UnityEngine;

public partial class PlayerInputController : TopDownController
{
    private Iinteraction _current; // ���� ��ȣ�ۿ� ������ ���

    [Header("UI Prompt")]
    [SerializeField] private GameObject fKeyPrompt; // �÷��̾� �ڽ��� FŰ ������ (�⺻ ��Ȱ��ȭ)

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        _current?.Interaction(transform);
        // ��ȣ�ۿ� ���Ŀ� ������Ʈ�� ��������/�������� ���⿡ ����:
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
