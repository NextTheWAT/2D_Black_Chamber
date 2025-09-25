using UnityEngine;
using UnityEngine.InputSystem;
using Constants;

public partial class PlayerInputController : TopDownController
{
    private bool shootPressed = false;

    private void Update()
    {
        // ����: ������ �ִ� ���� �� ������ �߻� �õ� (��ٿ��� Shooter�� �˾Ƽ�)
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
                GunForward(); // ������ �� ��� 1��
            }
            else if (ctx.phase == InputActionPhase.Canceled)
            {
                shootPressed = false;
                animationController.SetActiveShoot(false);
            }
        }
        else
        {
            if (!ctx.performed) return;   // ��-��
            animationController.SetActiveShoot(true);
            GunForward();
            animationController.SetActiveShoot(false);
        }
    }

    private void GunForward()
    {
        var cam = Camera.main;
        if (cam == null) return;

        if (shooter.gunPoint == null) return;

        // ���콺 �� ���� ��ǥ
        /*
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = shooter.gunPoint.position.z;
        Vector2 dir = ((Vector2)(mouse - shooter.gunPoint.position)).normalized;
        */

        if (shooter.Shoot())
        {
        }
        else
        {
            animationController.SetActiveShoot(false);
        }
        // ���� ��(��ٿ�/�� źâ)�� Shooter�� ����/��ٿ� ó����
    }
}
