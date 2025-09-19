using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    //[SerializeField] private CrosshairUI crosshair; // �ν����Ϳ��� �Ҵ�

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
                animationController.SetActiveShoot(true); // ���� �ִϸ��̼� ����
            }
            else if (ctx.phase == InputActionPhase.Canceled)
            {
                shootPressed = false;
                animationController.SetActiveShoot(false); // ���� �ִϸ��̼� ����
            }
        }
        else
        {
            if(!ctx.performed) return; // ���� ���� �߻�

            animationController.SetActiveShoot(true); // ���� �ִϸ��̼� ����
            animationController.SetActiveShoot(false); // ���� �ִϸ��̼� ����
            GunForward();
        }

        //crosshair?.Pulse();   // ũ�ν���� �޽�

        // ���� ������ ���߿� �߰�
    }

    private void GunForward()
    {
        var cam = Camera.main;
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = shooter.gunPoint.position.z;

        Vector2 dir = (mouse - shooter.gunPoint.position).normalized;

        if (shooter.Shoot(dir))
            UIManager.Instance.PulseCrosshair(); // �̱������� �����Ͽ� �޽� ȣ��
    }
}
