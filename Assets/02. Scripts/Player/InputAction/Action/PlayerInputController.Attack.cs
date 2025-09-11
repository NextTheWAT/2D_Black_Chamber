using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    //[SerializeField] private CrosshairUI crosshair; // 인스펙터에서 할당

    [SerializeField] Shooter shooter;



    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        //crosshair?.Pulse();   // 크로스헤어 펄스

        GunForward();

        UICrosshairManager.Instance.crosshairUI.Pulse(); // 싱글톤으로 접근하여 펄스 호출

        // 공격 로직은 나중에 추가
    }

    private void GunForward()
    {
        var cam = Camera.main;
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = shooter.gunPoint.position.z;

        Vector2 dir = (mouse - shooter.gunPoint.position).normalized;
        shooter.Shoot(dir); // ← 이 한 줄이면 발사
    }
}
