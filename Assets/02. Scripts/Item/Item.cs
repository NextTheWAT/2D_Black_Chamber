using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, Iinteraction
{
    public string itemName;
    public float pickupRange;  // �÷��̾� ���� ���� �Ÿ�

    private Transform player;    // �÷��̾� Transform
    private bool playerInRange = false; // �÷��̾ ��ó�� �ִ��� ����
    void Start()
    {
        // Inspector�� player�� �� ������ ������ �ڵ����� ã��
        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Scene �信�� ���� ǥ��
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
        Debug.Log(itemName + "�� �ֿ����ϴ�");

        //źâ �߰�
        Shooter shooter = player.GetComponentInChildren<Shooter>();
        if (shooter != null)
        {
            shooter.AddAmmo(12);
        }

        Destroy(gameObject);
    }

}



