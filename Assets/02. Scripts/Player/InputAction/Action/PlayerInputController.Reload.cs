using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    [SerializeField] private AudioClip reloadSfx;

    public void OnReload(InputAction.CallbackContext ctx)
    {
        animationController.PlayReload();
        AudioSource.PlayClipAtPoint(reloadSfx, transform.position);
    }
}
