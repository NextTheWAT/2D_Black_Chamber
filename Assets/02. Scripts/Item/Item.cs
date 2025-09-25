using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public float pickupRange;  // 플레이어 접근 감지 거리

    private Transform player;    // 플레이어 Transform
    private bool playerInRange = false; // 플레이어가 근처에 있는지 여부
    public GameObject Enemy;  //드랍 프리팹
    void Start()
    {
        // Inspector에 player를 안 넣으면 씬에서 자동으로 찾기
        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void Update()
    {
        if (!player) return;

        // 플레이어가 범위 안에 있는지 체크
        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= pickupRange;

        
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            PickUp();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Scene 뷰에서 범위 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }

    public void PickUp()
    {
        Debug.Log(itemName + "주웠습니다");
        Destroy(gameObject);
    }

}



