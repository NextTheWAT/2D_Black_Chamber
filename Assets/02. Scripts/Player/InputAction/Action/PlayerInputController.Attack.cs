using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    //[SerializeField] private CrosshairUI crosshair; // 인스펙터에서 할당

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        //crosshair?.Pulse();   // 크로스헤어 펄스

        UICrosshairManager.Instance.crosshairUI.Pulse(); // 싱글톤으로 접근하여 펄스 호출

        // 공격 로직은 나중에 추가
    }
}
