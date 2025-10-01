using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, Iinteraction
{
    public string itemName;
    public float pickupRange;  // 플레이어 접근 감지 거리

    private Transform player;    // 플레이어 Transform
    private bool playerInRange = false; // 플레이어가 근처에 있는지 여부
    void Start()
    {
        // Inspector에 player를 안 넣으면 씬에서 자동으로 찾기
        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Scene 뷰에서 범위 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
    
    public void Interaction()
    {
        if (playerInRange)
        {
            PickUp();
        }
    }
    public void PickUp()
    {
        Debug.Log(itemName + "을 주웠습니다");

        //탄창 추가
        Shooter shooter = player.GetComponentInChildren<Shooter>();
        if (shooter != null)
        {
            shooter.AddAmmo(12);
        }

        Destroy(gameObject);
    }

}



