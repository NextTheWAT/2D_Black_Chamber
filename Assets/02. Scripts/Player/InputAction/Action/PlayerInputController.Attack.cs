using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public partial class PlayerInputController : TopDownController
{
    [Header("Input Filtering")]
    [Tooltip("WorldSpace Canvas �� ������(?, !) ���� Ŭ���� ���� �ʰ� �Ϸ��� üũ")]
    [SerializeField] private bool ignoreWorldspaceCanvas = true;
    [Tooltip("���⿡ ������ ���̾��� UI�� '�߻� ����'���� �ν��մϴ�. ���� ���ͷ�Ƽ�� UI(Selectable)�� �����մϴ�.")]
    [SerializeField] private LayerMask blockingUILayers;

    // ListPool�� ���� Unity ������ ���� ���� ���� ���
    private static readonly List<RaycastResult> s_RaycastResults = new List<RaycastResult>(32);

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
        if (health.IsDead) return; // ��� �� ����
        if (shooter == null) return;

        // === UI ������ �˻� (���� ���� ������/�����ͷ�Ƽ�� �ؽ�Ʈ�� ���) ===
        if (IsPointerOverBlockingUI()) return;

        // 1) Ŭ�� ���� ����
        if (ctx.phase == InputActionPhase.Started)
        {
            // �Ѿ��� �ϳ��� ������: ��ġ 1ȸ
            if (!shooter.HasAnyAmmo)
            {
                animationController.PlayPunch();
                // TriggerPunchOnce(0.12f); // �ʿ��ϸ� �ð� ����
                return;
            }

            // ź�� ������ ����
            shootPressed = true;
            GunForward(); // ��� 1��
            return;
        }

        // 2) ��-�� ���(performed) ���� (respectFireRate=false�� ���� ����)
        if (!shooter.respectFireRate && ctx.performed)
        {
            if (!shooter.HasAnyAmmo)
            {
                animationController.PlayPunch();
                // TriggerPunchOnce(0.12f);
                return;
            }

            GunForward();
            return;
        }

        // 3) Ű/���콺 ���� ��
        if (ctx.phase == InputActionPhase.Canceled)
            shootPressed = false;
    }

    private bool IsPointerOverBlockingUI()
    {
        var es = EventSystem.current;
        if (es == null) return false;

        // ������ ��ġ
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

            // 1) CanvasGroup.blocksRaycasts == false �̸� Ŭ���� ���� �ʴ� UI �� ����
            var cg = go.GetComponentInParent<CanvasGroup>();
            if (cg != null && cg.blocksRaycasts == false) continue;

            // 2) WorldSpace Canvas�� ���� (�ɼ�)
            var canvas = go.GetComponentInParent<Canvas>();
            if (canvas != null && ignoreWorldspaceCanvas && canvas.renderMode == RenderMode.WorldSpace) continue;

            // 3) raycastTarget=false ��Ҵ� ���������� ���� ���� �� ����(������ġ)
            var g = go.GetComponent<Graphic>();
            if (g != null && g.raycastTarget == false) continue;

            // 4) Ư�� ���̾ ���� ������� ��� ���� ��
            if (blockingUILayers.value != 0)
            {
                if (((1 << go.layer) & blockingUILayers.value) == 0) continue;
            }
            else
            {
                // ���̾ ��� ������, "���ͷ�Ƽ�� UI"�� ����(��ư/�����̴� ��)
                if (go.GetComponentInParent<Selectable>() == null) continue;
            }

            // �� ������ ��� ����ϸ� ���� UI�� ����
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
            // ���� �߻�
            animationController.PlayShoot();
        }
        else
        {
            // ��ٿ�/��Ÿ ������ �̹߻� ��
        }
    }
    /*
    // ---- Punch 1ȸ �޽� ----
    private void TriggerPunchOnce(float holdTime)
    {
        if (punchCo != null) StopCoroutine(punchCo);
        punchCo = StartCoroutine(CoPunchPulse(holdTime));
    }

    private IEnumerator CoPunchPulse(float holdTime)
    {
        animationController.SetActivePunch(true);   // Punch BOOL ON
        yield return null;                          // �� ������ ����
        yield return new WaitForSeconds(holdTime);  // �ּ� ���� �ð�
        animationController.SetActivePunch(false);  // OFF
        punchCo = null;
    }
    */
}
