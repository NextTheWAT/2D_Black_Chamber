using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    private Iinteraction _current; // 지금 상호작용 가능한 대상

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        _current?.Interaction();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interaction"))
            _current = other.GetComponent<Iinteraction>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Interaction")) return;
        if (_current != null && _current == other.GetComponent<Iinteraction>())
            _current = null;
    }
}
