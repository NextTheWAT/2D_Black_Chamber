using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    //[SerializeField] private CrosshairUI crosshair; // 인스펙터에서 할당

    [SerializeField] Shooter shooter;

    private bool shootPressed = false;
    public bool ShootPressed => shootPressed;

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (shooter.respectFireRate)
        {
            if (ctx.phase == InputActionPhase.Started)
            {
                shootPressed = true;
                animationController.SetActiveShoot(true); // 공격 애니메이션 시작
            }
            else if (ctx.phase == InputActionPhase.Canceled)
            {
                shootPressed = false;
                animationController.SetActiveShoot(false); // 공격 애니메이션 종료
            }
        }
        else
        {
            if(!ctx.performed) return; // 누를 때만 발사

            animationController.SetActiveShoot(true); // 공격 애니메이션 시작
            animationController.SetActiveShoot(false); // 공격 애니메이션 종료
            GunForward();
        }

        //crosshair?.Pulse();   // 크로스헤어 펄스

        // 공격 로직은 나중에 추가
    }

    private void GunForward()
    {
        var cam = Camera.main;
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = shooter.gunPoint.position.z;

        Vector2 dir = (mouse - shooter.gunPoint.position).normalized;

        if (shooter.Shoot(dir))
            UIManager.Instance.PulseCrosshair(); // 싱글톤으로 접근하여 펄스 호출
    }
}
