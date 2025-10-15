using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    public float aimPingPongMaxAngle = 20f; // �ִ� ���� ����
    public float aimPingPongMinAngle = 10f; // �ּ� ���� ����
    private float currentPingPongAngle;

    public float aimPingPongMaxSpeed = 5f; // �ִ� ���� �ӵ�
    public float aimPingPongMinSpeed = 2f; // �ּ� ���� �ӵ�
    private float currentPingPongSpeed;
    public float aimingTransitionSpeed = 5f;

    public float aimingDuration = 5f; // ���� ���� �ð�
    private float currentAimingTime = 0f;

    public float aimLineDistance = 1.5f;

    public float aimRunningSpeedPenalty = 10f; // �޸��� �� ���� ��Ȯ�� �г�Ƽ

    private Transform aimLineTransform;
    private LineRenderer lineRenderer;

    private float penaltyAngle = 0f; // �г�Ƽ ����
    private float penaltySpeed = 0f; // �г�Ƽ �ӵ�

    private Shooter CurrentShooter => WeaponManager.Instance.CurrentWeapon;

    private bool isAiming = false;
    private float pingpongTime = 0f;
    private bool aimingCooldown = false;

    private PlayerInputController inputController;

    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        inputController = GetComponent<PlayerInputController>();
        aimLineTransform = lineRenderer.transform;
    }

    void Update()
    {
        isAiming = aimingCooldown ? false : Input.GetMouseButton(1);
        UpdateAimingTime();
        UpdatePenalty();
        UpdateAimParameters();
        UpdateAimLine();
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

        targetAngle += penaltyAngle;
        targetSpeed += penaltySpeed;

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
