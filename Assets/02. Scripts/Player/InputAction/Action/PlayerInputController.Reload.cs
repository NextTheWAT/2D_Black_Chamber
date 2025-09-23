using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{

    public void OnReload(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        animationController.PlayReload();
        WeaponManager.Instance.RequestReload();
    }
}
