using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    public void OnReload(InputAction.CallbackContext ctx)
    {
        animationController.PlayReload();
        WeaponSoundManager.Instance.PlayReloadSound(); // 재장전 사운드 재생
    }
}
