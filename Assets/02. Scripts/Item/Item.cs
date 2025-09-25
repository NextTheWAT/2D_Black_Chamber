using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public float pickupRange;  // �÷��̾� ���� ���� �Ÿ�

    private Transform player;    // �÷��̾� Transform
    private bool playerInRange = false; // �÷��̾ ��ó�� �ִ��� ����
    public GameObject Enemy;  //��� ������
    void Start()
    {
        // Inspector�� player�� �� ������ ������ �ڵ����� ã��
        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void Update()
    {
        if (!player) return;

        // �÷��̾ ���� �ȿ� �ִ��� üũ
        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= pickupRange;

        
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            PickUp();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Scene �信�� ���� ǥ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }

    public void PickUp()
    {
        Debug.Log(itemName + "�ֿ����ϴ�");
        Destroy(gameObject);
    }

}



