using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{

    public void OnSwitch(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) {

            animationController.PlaySwitch();
        }
    }
}
