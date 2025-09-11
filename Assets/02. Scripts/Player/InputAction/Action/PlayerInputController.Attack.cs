using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    //[SerializeField] private CrosshairUI crosshair; // �ν����Ϳ��� �Ҵ�

    [SerializeField] Shooter shooter;



    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        //crosshair?.Pulse();   // ũ�ν���� �޽�

        GunForward();

        UICrosshairManager.Instance.crosshairUI.Pulse(); // �̱������� �����Ͽ� �޽� ȣ��

        // ���� ������ ���߿� �߰�
    }

    private void GunForward()
    {
        var cam = Camera.main;
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = shooter.gunPoint.position.z;

        Vector2 dir = (mouse - shooter.gunPoint.position).normalized;
        shooter.Shoot(dir); // �� �� �� ���̸� �߻�
    }
}
