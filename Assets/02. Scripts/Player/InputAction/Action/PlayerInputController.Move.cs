using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{

    // Move : 2D Vector (WASD)
    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 move = ctx.ReadValue<Vector2>();
        if (move.sqrMagnitude > 1f) move.Normalize();
        CallMoveEvent(move);

        if (ctx.canceled) CallMoveEvent(Vector2.zero);
    }

}
