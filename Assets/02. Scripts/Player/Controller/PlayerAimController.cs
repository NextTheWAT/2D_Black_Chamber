using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    [Header("Line")]
    public LineRenderer aimLineRenderer;
    public LineRenderer leftLineRenderer;
    public LineRenderer rightLineRenderer;
    public float aimMaxWidth = 0.1f; // �ִ� ���ؼ� �ʺ�
    public float aimMinWidth = 0.02f; // �ּ� ���ؼ� �ʺ�
    private float currentAimWidth;
    private Transform aimLineTransform;

    [Header("Arc")]
    public ArcDrawer gaugeArcDrawer;
    public ArcDrawer backgroundArcDrawer;

    public Color gaugeOriginalColor; // ���� ����
    public Color gaugeCooldownColor; // ��Ÿ�� ����

    public float maxAlpha = 0.8f; // �ִ� ���� ��
    public float gaugeColorTransitionSpeed = 5f; // ���� ��ȯ �ӵ�

    [Header("Angle")]
    public float aimPingPongMaxAngle = 20f; // �ִ� ���� ����
    public float aimPingPongMinAngle = 10f; // �ּ� ���� ����
    private float currentPingPongAngle;

    [Header("Speed")]
    public float aimPingPongMaxSpeed = 5f; // �ִ� ���� �ӵ�
    public float aimPingPongMinSpeed = 2f; // �ּ� ���� �ӵ�
    private float currentPingPongSpeed;
    public float aimingTransitionSpeed = 5f;

    public float aimLineOffset = 0.2f;
    public float aimLineDistance = 1.5f;

    public float sideLineOffset = 1f;
    public float sideLineDistance = 1f;

    [Header("Aiming")]
    public float aimingDuration = 5f; // ���� ���� �ð�
    private float currentAimingTime = 0f;

    [Header("Penalty")]
    public float aimRunningSpeedPenalty = 10f; // �޸��� �� ���� ��Ȯ�� �г�Ƽ

    private float penaltyAngle = 0f; // �г�Ƽ ����
    private float penaltySpeed = 0f; // �г�Ƽ �ӵ�

    private Shooter CurrentShooter => WeaponManager.Instance.CurrentWeapon;

    private bool isAiming = false;
    private float pingpongTime = 0f;
    private bool aimingCooldown = false;

    private PlayerInputController inputController;

    void Start()
    {
        inputController = GetComponent<PlayerInputController>();
        aimLineTransform = aimLineRenderer.transform;

        leftLineRenderer.positionCount = 2;
        rightLineRenderer.positionCount = 2;
    }

    void LateUpdate()
    {
        isAiming = aimingCooldown ? false : Input.GetMouseButton(1);
        UpdateAimingTime();
        UpdatePenalty();
        UpdateAimParameters();
        UpdateAimLine();
        UpdateSideLine();
        DrawArc();
    }
    void DrawArc()
    {
        float ratio = currentAimingTime / aimingDuration;
        float aimingGauge = 1 - ratio;
        Color targetColor = aimingCooldown ? gaugeCooldownColor : Color.Lerp(gaugeOriginalColor, gaugeCooldownColor, ratio);
        Color gaugeColor = Color.Lerp(gaugeArcDrawer.GetColor(), targetColor, Time.deltaTime * gaugeColorTransitionSpeed);

        gaugeArcDrawer.transform.position = CurrentShooter.gunPoint.position;
        gaugeArcDrawer.DrawArc(currentPingPongAngle * aimingGauge);
        gaugeArcDrawer.SetAlpha(aimingGauge * maxAlpha);
        gaugeArcDrawer.SetColor(gaugeColor);

        backgroundArcDrawer.transform.position = CurrentShooter.gunPoint.position;
        backgroundArcDrawer.DrawArc(currentPingPongAngle);
    }

    void UpdatePenalty()
    {
        penaltySpeed = inputController.RunHeld ? aimRunningSpeedPenalty : 0f;
    }

    void UpdateAimingTime()
    {
        float aimingTimeDelta = isAiming ? Time.deltaTime : -Time.deltaTime;
        currentAimingTime = Mathf.Clamp(currentAimingTime + aimingTimeDelta, 0f, aimingDuration);

        if (currentAimingTime >= aimingDuration)
        {
            aimingCooldown = true;
        }
        else if (currentAimingTime <= 0f)
        {
            aimingCooldown = false;
        }
    }

    void UpdateAimParameters()
    {
        float targetAngle = isAiming ? aimPingPongMinAngle : aimPingPongMaxAngle;
        float targetSpeed = isAiming ? aimPingPongMinSpeed : aimPingPongMaxSpeed;
        float targetWidth = isAiming ? aimMinWidth : aimMaxWidth;

        targetAngle += penaltyAngle;
        targetSpeed += penaltySpeed;

        currentPingPongAngle = Mathf.Lerp(currentPingPongAngle, targetAngle, aimingTransitionSpeed * Time.deltaTime);
        currentPingPongSpeed = Mathf.Lerp(currentPingPongSpeed, targetSpeed, aimingTransitionSpeed * Time.deltaTime);
        currentAimWidth = Mathf.Lerp(currentAimWidth, targetWidth, Time.deltaTime);
    }

    void UpdateAimLine()
    {
        if (aimLineRenderer == null) return;

        pingpongTime += Time.deltaTime * currentPingPongSpeed;
        float aimAngle = Mathf.Sin(pingpongTime) * currentPingPongAngle * 0.5f;
        aimLineTransform.localRotation = Quaternion.Euler(0f, 0f, aimAngle);
        CurrentShooter.gunPoint.transform.localRotation = aimLineTransform.localRotation;

        Vector3 startPos = CurrentShooter.gunPoint.transform.position + aimLineTransform.up * aimLineOffset;
        Vector3 endPos = startPos + aimLineTransform.up * aimLineDistance;

        aimLineRenderer.SetPosition(0, startPos);
        aimLineRenderer.SetPosition(1, endPos);

        aimLineRenderer.startWidth = currentAimWidth;
        aimLineRenderer.endWidth = currentAimWidth;
    }

    void UpdateSideLine()
    {
        leftLineRenderer.transform.position = CurrentShooter.gunPoint.position;
        rightLineRenderer.transform.position = CurrentShooter.gunPoint.position;

        float leftRad = (currentPingPongAngle * 0.5f + 90f) * Mathf.Deg2Rad;
        float rightRad = (-currentPingPongAngle * 0.5f + 90f) * Mathf.Deg2Rad;

        Vector2 leftDir = new(Mathf.Cos(leftRad), Mathf.Sin(leftRad));
        Vector2 rightDir = new(Mathf.Cos(rightRad), Mathf.Sin(rightRad));

        Vector3 leftStartPos = (Vector3)(leftDir * sideLineOffset);
        Vector3 rightStartPos = (Vector3)(rightDir * sideLineOffset);
        Vector3 leftEndPos = (Vector3)(leftDir * (sideLineOffset + sideLineDistance));
        Vector3 rightEndPos = (Vector3)(rightDir * (sideLineOffset + sideLineDistance));

        leftLineRenderer.SetPosition(0, leftStartPos);
        leftLineRenderer.SetPosition(1, leftEndPos);
        rightLineRenderer.SetPosition(0, rightStartPos);
        rightLineRenderer.SetPosition(1, rightEndPos);
    }

}
