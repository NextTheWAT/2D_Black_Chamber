using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 1.0f;
    private bool isOpen = false;
    private bool playerInRange = false; // 플레이어가 범위안에 있을때 상호작용하게 하기 위해

    private Quaternion closeRotation;   // 각각의 회전값
    private Quaternion openRotation;

    private void Start()
    {

        closeRotation = transform.rotation;    // 처음 닫혀있을때 회전값 저장
        openRotation = Quaternion.Euler(0, 0, openAngle) * closeRotation;   // 열렸을때 회전값도 저장

        transform.rotation = closeRotation;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        Quaternion targetRotation = isOpen ? openRotation : closeRotation;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, openSpeed * Time.deltaTime * 100);
        // RotateTowards : 현재 회전 -> 목표 회전으로 계산
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            playerInRange = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }
}
