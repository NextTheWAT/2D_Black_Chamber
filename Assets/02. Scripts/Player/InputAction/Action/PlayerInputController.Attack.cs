using UnityEngine;
using UnityEngine.InputSystem;
using Constants;
using System.Collections;
public partial class PlayerInputController : TopDownController
{
    private bool shootPressed = false;
    private Coroutine punchCo;

    private void Update()
    {
        // ����: ������ �ִ� ���� �� ������ �߻� �õ�
        if (shootPressed && shooter != null)
        {
            // ź�� �ϳ��� ������ Ȧ�� �߿��� �ƹ� �͵� ���� ���� (Ŭ�� �������� ��ġ)
            if (!shooter.HasAnyAmmo) return;

            GunForward();
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (shooter == null) return;

        // 1) Ŭ�� ���� ����
        if (ctx.phase == InputActionPhase.Started)
        {
            // �Ѿ��� �ϳ��� ������: ��ġ 1ȸ
            if (!shooter.HasAnyAmmo)
            {
                TriggerPunchOnce(0.2f); // �ʿ��ϸ� �ð� ����
                return;
            }

            // ź�� ������ ����
            shootPressed = true;
            animationController.SetActiveShoot(true);
            GunForward(); // ��� 1��
            return;
        }

        // 2) ��-�� ���(performed) ���� (respectFireRate=false�� ���� ����)
        if (!shooter.respectFireRate && ctx.performed)
        {
            if (!shooter.HasAnyAmmo)
            {
                TriggerPunchOnce(0.12f);
                return;
            }

            animationController.SetActiveShoot(true);
            GunForward();
            animationController.SetActiveShoot(false);
            return;
        }

        // 3) Ű/���콺 ���� ��
        if (ctx.phase == InputActionPhase.Canceled)
        {
            shootPressed = false;
            animationController.SetActiveShoot(false);
        }
    }

    private void GunForward()
    {
        var cam = Camera.main;
        if (cam == null || shooter.gunPoint == null) return;

        if (shooter.Shoot())
        {
            // ���� �߻�
        }
        else
        {
            // ��ٿ�/��Ÿ ������ �̹߻� ��
            animationController.SetActiveShoot(false);
        }
    }

    // ---- Punch 1ȸ �޽� ----
    private void TriggerPunchOnce(float holdTime)
    {
        if (punchCo != null) StopCoroutine(punchCo);
        punchCo = StartCoroutine(CoPunchPulse(holdTime));
    }

    private IEnumerator CoPunchPulse(float holdTime)
    {
        animationController.SetActiveShoot(false);  // ���� �ִ� ����(�浹 ����)
        animationController.SetActivePunch(true);   // Punch BOOL ON
        yield return null;                          // �� ������ ����
        yield return new WaitForSeconds(holdTime);  // �ּ� ���� �ð�
        animationController.SetActivePunch(false);  // OFF
        punchCo = null;
    }
}