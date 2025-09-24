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
            Vector2 doorPosition = new Vector2(transform.position.x, transform.position.y);    // 문 위치
            Vector2 playerPosition = new Vector2(player.position.x, player.position.y);        // 플레이어 위치
            Vector2 direction = (playerPosition - doorPosition);

            Vector2 localDirection = transform.InverseTransformPoint(player.position);

            float angle = (localDirection.x >= 0) ? -90f : 90f;

            targetRotation = closeRotation * Quaternion.Euler(0, 0, angle);
        }
        else
        {
            targetRotation = closeRotation;
        }

        isOpen = !isOpen;
    }

}
