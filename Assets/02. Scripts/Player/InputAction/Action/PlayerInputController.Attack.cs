using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public partial class PlayerInputController : TopDownController
{
    [Header("Input Filtering")]
    [Tooltip("WorldSpace Canvas 위 아이콘(?, !) 등은 클릭을 막지 않게 하려면 체크")]
    [SerializeField] private bool ignoreWorldspaceCanvas = true;
    [Tooltip("여기에 지정된 레이어의 UI만 '발사 차단'으로 인식합니다. 비우면 인터랙티브 UI(Selectable)만 차단합니다.")]
    [SerializeField] private LayerMask blockingUILayers;

    // ListPool이 없는 Unity 버전을 위해 고정 버퍼 사용
    private static readonly List<RaycastResult> s_RaycastResults = new List<RaycastResult>(32);

    private bool shootPressed = false;

    private void Update()
    {
        // 연사: 누르고 있는 동안 매 프레임 발사 시도
        if (shootPressed && shooter != null && !WeaponManager.Instance.isReloading)
        {
            // 탄이 하나도 없으면 홀드 중에는 아무 것도 하지 않음 (클릭 순간에만 펀치)
            if (!shooter.HasAnyAmmo) return;

            GunForward();
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        // === UI 위인지 검사 (월드 공간 아이콘/비인터랙티브 텍스트는 통과) ===
        if (IsPointerOverBlockingUI()) return;
        if (health.IsDead) return; // 사망 시 무시
        if (shooter == null) return;

        // 1) 클릭 시작 시점
        if (ctx.phase == InputActionPhase.Started)
        {
            // 총알이 하나도 없으면: 펀치 1회
            if (!shooter.HasAnyAmmo)
            {
                animationController.PlayPunch();
                return;
            }

            if (WeaponManager.Instance.isReloading) return;
            // 탄이 있으면 슈팅
            GunForward(); // 즉시 1발
            shootPressed = true;
            return;
        }

        // 2) 탭-샷 모드(performed) 대응 (respectFireRate=false일 때도 안전)
        if (!shooter.respectFireRate && ctx.performed)
        {
            if (!shooter.HasAnyAmmo)
            {
                animationController.PlayPunch();
                return;
            }

            if (WeaponManager.Instance.isReloading) return;
            GunForward();
            return;
        }

        // 3) 클릭 종료 시점
        if (ctx.phase == InputActionPhase.Canceled)
            shootPressed = false;
    }

    private bool IsPointerOverBlockingUI()
    {
        var es = EventSystem.current;
        if (es == null) return false;

        // 포인터 위치
        Vector2 pos = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        var data = new PointerEventData(es) { position = pos };

        s_RaycastResults.Clear();
        es.RaycastAll(data, s_RaycastResults);

        bool blocked = false;
        for (int i = 0; i < s_RaycastResults.Count; i++)
        {
            var r = s_RaycastResults[i];
            var go = r.gameObject;
            if (go == null) continue;

            // 1) CanvasGroup.blocksRaycasts == false 이면 클릭을 막지 않는 UI → 무시
            var cg = go.GetComponentInParent<CanvasGroup>();
            if (cg != null && cg.blocksRaycasts == false) continue;

            // 2) WorldSpace Canvas는 무시 (옵션)
            var canvas = go.GetComponentInParent<Canvas>();
            if (canvas != null && ignoreWorldspaceCanvas && canvas.renderMode == RenderMode.WorldSpace) continue;

            // 3) raycastTarget=false 요소는 실질적으로 막지 않음 → 무시(안전장치)
            var g = go.GetComponent<Graphic>();
            if (g != null && g.raycastTarget == false) continue;

            // 4) 특정 레이어만 차단 대상으로 삼고 싶을 때
            if (blockingUILayers.value != 0)
            {
                if (((1 << go.layer) & blockingUILayers.value) == 0) continue;
            }
            else
            {
                // 레이어가 비어 있으면, "인터랙티브 UI"만 차단(버튼/슬라이더 등)
                if (go.GetComponentInParent<Selectable>() == null) continue;
            }

            // 위 조건을 모두 통과하면 차단 UI로 간주
            blocked = true;
            break;
        }

        s_RaycastResults.Clear();
        return blocked;
    }

    private void GunForward()
    {
        var cam = Camera.main;
        if (cam == null || shooter.gunPoint == null) return;

        if (shooter.Shoot())
        {
            // 정상 발사
            animationController.PlayShoot();
            cameraController.ShakeCamera(shooter.gunData.recoilAmount);
        }
        else
        {
            // 쿨다운/기타 사유로 미발사 시
        }
    }

}
