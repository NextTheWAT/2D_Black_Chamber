using UnityEngine;
using UnityEngine.InputSystem;
using Constants;

public partial class PlayerInputController : TopDownController
{
    private bool shootPressed = false;

    private void Update()
    {
        // 연사: 누르고 있는 동안 매 프레임 발사 시도 (쿨다운은 Shooter가 알아서)
        if (shootPressed && shooter != null)
        {
            GunForward();
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (shooter == null) return;

        if (shooter.respectFireRate)
        {
            if (ctx.phase == InputActionPhase.Started)
            {
                shootPressed = true;
                animationController.SetActiveShoot(true);
                GunForward(); // 눌렀을 때 즉시 1발
            }
            else if (ctx.phase == InputActionPhase.Canceled)
            {
                shootPressed = false;
                animationController.SetActiveShoot(false);
            }
        }
        else
        {
            if (!ctx.performed) return;   // 탭-샷
            animationController.SetActiveShoot(true);
            GunForward();
            animationController.SetActiveShoot(false);
        }
    }

    private void GunForward()
    {
        var cam = Camera.main;
        if (cam == null) return;

        // 현재 무기 타입 기준으로 총구 선택
        var wm = WeaponManager.Instance;
        var type = wm ? wm.CurrentWeapon : WeaponType.Pistol;

        Transform muzzle = (type == WeaponType.Rifle)
            ? shooter.gunRiflePoint
            : shooter.gunPistolPoint;

        if (muzzle == null) return;

        // 마우스 → 월드 좌표
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = muzzle.position.z;

        Vector2 dir = ((Vector2)(mouse - muzzle.position)).normalized;

        if (shooter.Shoot(dir))
        {
        }
        // 실패 시(쿨다운/빈 탄창)는 Shooter가 사운드/쿨다운 처리함
    }
}
