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
        // 연사: 누르고 있는 동안 매 프레임 발사 시도
        if (shootPressed && shooter != null)
        {
            // 탄이 하나도 없으면 홀드 중에는 아무 것도 하지 않음 (클릭 순간에만 펀치)
            if (!shooter.HasAnyAmmo) return;

            GunForward();
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (shooter == null) return;

        // 1) 클릭 시작 시점
        if (ctx.phase == InputActionPhase.Started)
        {
            // 총알이 하나도 없으면: 펀치 1회
            if (!shooter.HasAnyAmmo)
            {
                TriggerPunchOnce(0.2f); // 필요하면 시간 조절
                return;
            }

            // 탄이 있으면 슈팅
            shootPressed = true;
            animationController.SetActiveShoot(true);
            GunForward(); // 즉시 1발
            return;
        }

        // 2) 탭-샷 모드(performed) 대응 (respectFireRate=false일 때도 안전)
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

        // 3) 키/마우스 뗐을 때
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
            // 정상 발사
        }
        else
        {
            // 쿨다운/기타 사유로 미발사 시
            animationController.SetActiveShoot(false);
        }
    }

    // ---- Punch 1회 펄스 ----
    private void TriggerPunchOnce(float holdTime)
    {
        if (punchCo != null) StopCoroutine(punchCo);
        punchCo = StartCoroutine(CoPunchPulse(holdTime));
    }

    private IEnumerator CoPunchPulse(float holdTime)
    {
        animationController.SetActiveShoot(false);  // 슈팅 애니 끄기(충돌 방지)
        animationController.SetActivePunch(true);   // Punch BOOL ON
        yield return null;                          // 한 프레임 보장
        yield return new WaitForSeconds(holdTime);  // 최소 유지 시간
        animationController.SetActivePunch(false);  // OFF
        punchCo = null;
    }
}