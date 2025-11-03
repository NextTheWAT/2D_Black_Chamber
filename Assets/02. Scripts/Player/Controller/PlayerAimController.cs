using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    public Action<bool> OnAimingChanged;

    [Header("Line")]
    public LineRenderer aimLineRenderer;
    public LineRenderer leftLineRenderer;
    public LineRenderer rightLineRenderer;

    public float aimMaxWidth = 0.1f; // 최대 조준선 너비
    public float aimMinWidth = 0.02f; // 최소 조준선 너비

    public float aimLineOffset = 0.2f;
    public float sideLineOffset = 1f;

    private float currentAimWidth;
    private Transform aimLineTransform;

    [Header("Arc")]
    public ArcDrawer gaugeArcDrawer;
    public ArcDrawer backgroundArcDrawer;

    public Color gaugeOriginalColor; // 원래 색상
    public Color gaugeCooldownColor; // 쿨타임 색상

    public float maxAlpha = 0.8f; // 최대 알파 값
    public float gaugeColorTransitionSpeed = 5f; // 색상 전환 속도


    [Header("Accuracy")]
    public float accuracy = 1.5f; // 조준선 길이 - 높을수록 조준선이 길어짐
    public float aimAccuracy = 3f; // 조준 시 조준선 길이
    public float sideLineDistance = 2f; // 조준 시 사이드 조준선 길이
    private float currentAccuracy;

    [Header("Precision")]
    public float precision = 20f; // 왕복하는 범위
    public float aimPrecision = 10f; // 조준 시 왕복하는 범위
    private float currentPrecision;

    [Header("Speed")]
    public float stability = 5f; // 조준선이 왕복 운동하는 속도 - 높을수록 속도가 줄어듬
    public float aimStability = 2f; // 최소 핑퐁 속도
    private float currentStability;
    public float aimingTransitionSpeed = 5f;

    [Header("Aiming")]
    public float aimingDuration = 5f; // 조준 지속 시간
    private float currentAimingTime = 0f;

    [Header("Penalty")]
    public float aimRunningSpeedPenalty = 10f; // 달리기 시 조준 정확도 패널티

    private float penaltyAngle = 0f; // 패널티 각도
    private float penaltySpeed = 0f; // 패널티 속도

    private Shooter CurrentShooter => WeaponManager.Instance.CurrentWeapon;
    private GunData GunData
    {
        get
        {
            if(CurrentShooter == null) return null;
            return CurrentShooter.gunData;
        }
    }

    private bool isAiming = false;
    private float pingpongTime = 0f;
    private bool aimingCooldown = false;

    private PlayerInputController inputController;
    private float originalCameraSize;

    void Start()
    {
        inputController = GetComponent<PlayerInputController>();
        originalCameraSize = Camera.main.orthographicSize;
        EnsureCachedTransforms();

        if (leftLineRenderer) leftLineRenderer.positionCount = 2;
        if (rightLineRenderer) rightLineRenderer.positionCount = 2;
    }

    void LateUpdate()
    {
        EnsureCachedTransforms();

        InputAiming();
        UpdateAimingTime();
        UpdatePenalty();
        UpdateAimParameters();

        // 총기가 아직 없으면 라인/게이지 갱신 스킵
        if (!TryGetGunPoint(out _)) return;

        UpdateAimLine();
        UpdateSideLine();
        // UpdateCameraSize();
        DrawArc();
    }

    void InputAiming()
    {
        bool previousAiming = isAiming;
        isAiming = aimingCooldown ? false : Input.GetMouseButton(1);
        if(previousAiming != isAiming)
            OnAimingChanged?.Invoke(isAiming);
    }

    void DrawArc()
    {
        if (gaugeArcDrawer == null || backgroundArcDrawer == null) return;
        if (!TryGetGunPoint(out var gunPoint)) return;

        float ratio = currentAimingTime / aimingDuration;
        float aimingGauge = 1 - ratio;
        Color targetColor = aimingCooldown ? gaugeCooldownColor : Color.Lerp(gaugeOriginalColor, gaugeCooldownColor, ratio);
        Color gaugeColor = Color.Lerp(gaugeArcDrawer.GetColor(), targetColor, Time.deltaTime * gaugeColorTransitionSpeed);

        gaugeArcDrawer.transform.position = gunPoint.position;
        gaugeArcDrawer.DrawArc(precision * aimingGauge);
        gaugeArcDrawer.SetAlpha(aimingGauge * maxAlpha);
        gaugeArcDrawer.SetColor(gaugeColor);

        backgroundArcDrawer.transform.position = gunPoint.position;
        backgroundArcDrawer.DrawArc(precision);
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
        if (GunData == null) return;
        float targetAngle = isAiming ? GunData.aimPrecision : GunData.precision;
        float targetStability = isAiming ? GunData.aimStability : GunData.stability;
        float targetWidth = isAiming ? aimMinWidth : aimMaxWidth;
        float targetAccuracy = isAiming ? GunData.aimAccuracy : GunData.accuracy;

        targetAngle += penaltyAngle;
        targetStability += penaltySpeed;

        currentPrecision = Mathf.Lerp(currentPrecision, targetAngle, aimingTransitionSpeed * Time.deltaTime);
        currentStability = Mathf.Lerp(currentStability, targetStability, aimingTransitionSpeed * Time.deltaTime);
        currentAimWidth = Mathf.Lerp(currentAimWidth, targetWidth, Time.deltaTime);
        currentAccuracy = Mathf.Lerp(currentAccuracy, targetAccuracy, Time.deltaTime * 10f);
    }

    void UpdateAimLine()
    {
        if (GunData == null) return;
        if (aimLineRenderer == null || aimLineTransform == null) return;
        if (!TryGetGunPoint(out var gunPoint)) return;

        float shakeSpeed = 10f / Mathf.Max(currentStability, 0.01f);
        pingpongTime += Time.deltaTime * shakeSpeed;
        float aimAngle = Mathf.Sin(pingpongTime) * currentPrecision * 0.5f;

        // 라인 기준 회전 ↔ 총기 기준 회전 동기화
        aimLineTransform.localRotation = Quaternion.Euler(0f, 0f, aimAngle);
        gunPoint.localRotation = aimLineTransform.localRotation;

        Vector3 startPos = gunPoint.position + aimLineTransform.up * aimLineOffset;
        Vector3 endPos = startPos + aimLineTransform.up * currentAccuracy;

        aimLineRenderer.SetPosition(0, startPos);
        aimLineRenderer.SetPosition(1, endPos);

        aimLineRenderer.startWidth = currentAimWidth;
        aimLineRenderer.endWidth = currentAimWidth;
    }

    void UpdateSideLine()
    {
        if (!TryGetGunPoint(out var gunPoint)) return;
        if (!leftLineRenderer || !rightLineRenderer) return;

        // 라인 기준점(Transform)을 총구 위치로 맞춘 뒤…
        leftLineRenderer.transform.position = gunPoint.position;
        rightLineRenderer.transform.position = gunPoint.position;

        float leftRad = (currentPrecision * 0.5f + 90f) * Mathf.Deg2Rad;
        float rightRad = (-currentPrecision * 0.5f + 90f) * Mathf.Deg2Rad;

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

    void UpdateCameraSize()
    {
        float targetSize = isAiming ? originalCameraSize * GunData.aimDistance : originalCameraSize;
        float currentSize = Mathf.Lerp(Camera.main.orthographicSize, targetSize, Time.deltaTime * 5f);
        Camera.main.orthographicSize = currentSize;
    }


    private bool TryGetGunPoint(out Transform gunPoint)
    {
        gunPoint = null;
        var wm = WeaponManager.Instance;
        var shooter = (wm != null) ? wm.CurrentWeapon : null;
        if (shooter == null || shooter.gunPoint == null) return false;
        gunPoint = shooter.gunPoint;
        return true;
    }

    private void EnsureCachedTransforms()
    {
        if (aimLineRenderer != null && aimLineTransform == null)
            aimLineTransform = aimLineRenderer.transform;
    }

}
