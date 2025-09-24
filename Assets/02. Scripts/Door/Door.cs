using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Iinteraction
{
    //public float openAngle = 90f;
    public float openSpeed = 1.0f;
    private bool isOpen = false;
    //private bool playerInRange = false; // 플레이어가 범위안에 있을때 상호작용하게 하기 위해

    private Quaternion closeRotation;   // 각각의 회전값
    private Quaternion targetRotation;

    public Transform player;

    private void Start()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        closeRotation = transform.rotation;    // 처음 닫혀있을때 회전값 저장
        targetRotation = closeRotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, openSpeed * Time.deltaTime * 100);
        // RotateTowards : 현재 회전 -> 목표 회전으로 계산
    }

    public void Interaction()
    {
        if (!isOpen)
        {
            Vector3 playerPosition = (player.position - transform.position).normalized; // 플레이어가 어디 서있는지 판단
            float dot = Vector3.Dot(transform.forward, playerPosition); // 내적 계산으로 플레이어가 앞인지 뒤인지

            float angle = (dot > 0) ? 90f : -90f;

            Vector3 openPosition = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;

            targetRotation = Quaternion.LookRotation(openPosition, Vector3.up);
        }
        else
        {
            targetRotation = closeRotation;
        }

        isOpen = !isOpen;
    }

}
