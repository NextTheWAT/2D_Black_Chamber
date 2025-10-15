using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    public float aimPingPongMaxAngle = 20f;
    public float aimPingPongSpeed = 5f;
    public float aimLineDistance = 1.5f;

    private float aimAngle = 0f;
    private Transform aimLineTransform;
    private LineRenderer lineRenderer;

    private Shooter CurrentShooter => WeaponManager.Instance.CurrentWeapon;

    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        aimLineTransform = lineRenderer.transform;
    }

    void Update()
    {
        AimRotationPingPong();
    }

    void AimRotationPingPong()
    {
        if (lineRenderer == null) return;

        aimAngle = Mathf.Sin(Time.time * aimPingPongSpeed) * aimPingPongMaxAngle;
        aimLineTransform.localRotation = Quaternion.Euler(0f, 0f, aimAngle);
        CurrentShooter.gunPoint.transform.localRotation = aimLineTransform.localRotation;

        Vector3 startPos = CurrentShooter.gunPoint.transform.position;
        Vector3 endPos = startPos + aimLineTransform.up * aimLineDistance;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

}
