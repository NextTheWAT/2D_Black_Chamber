using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    public float aimPingPongMaxAngle = 20f; // 최대 핑퐁 범위
    public float aimPingPongMinAngle = 10f; // 최소 핑퐁 범위
    private float currentPingPongAngle;

    public float aimPingPongMaxSpeed = 5f; // 최대 핑퐁 속도
    public float aimPingPongMinSpeed = 2f; // 최소 핑퐁 속도
    private float currentPingPongSpeed;
    public float aimingTransitionSpeed = 5f;

    public float aimingDuration = 5f; // 조준 지속 시간
    private float currentAimingTime = 0f;



    public float aimLineDistance = 1.5f;

    private Transform aimLineTransform;
    private LineRenderer lineRenderer;

    private Shooter CurrentShooter => WeaponManager.Instance.CurrentWeapon;

    private bool isAiming = false;
    private float pingpongTime = 0f;
    private bool aimingCooldown = false;

    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        aimLineTransform = lineRenderer.transform;
    }

    void Update()
    {
        isAiming = aimingCooldown ? false : Input.GetMouseButton(1);
        UpdateAimingTime();
        UpdateAimParameters();
        UpdateAimLine();
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

        currentPingPongAngle = Mathf.MoveTowards(currentPingPongAngle, targetAngle, aimingTransitionSpeed * Time.deltaTime);
        currentPingPongSpeed = Mathf.MoveTowards(currentPingPongSpeed, targetSpeed, aimingTransitionSpeed * Time.deltaTime);
    }

    void UpdateAimLine()
    {
        if (lineRenderer == null) return;

        pingpongTime += Time.deltaTime * currentPingPongSpeed;
        float aimAngle = Mathf.Sin(pingpongTime) * currentPingPongAngle;
        aimLineTransform.localRotation = Quaternion.Euler(0f, 0f, aimAngle);
        CurrentShooter.gunPoint.transform.localRotation = aimLineTransform.localRotation;

        Vector3 startPos = CurrentShooter.gunPoint.transform.position;
        Vector3 endPos = startPos + aimLineTransform.up * aimLineDistance;



        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

}
