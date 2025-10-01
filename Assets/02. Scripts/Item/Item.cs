using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, Iinteraction
{
    public enum WeaponType  // ���� Ÿ��
    {
        Gun,
        Rifle
    }

    public string itemName;
    public WeaponType weaponType; // ����/������ ����
    public int ammoAmount;

    private GameObject player;    // �÷��̾� Transform
    //private bool playerInRange = false; // �÷��̾ ��ó�� �ִ��� ����
    void Start()
    {
        // Inspector�� player�� �� ������ ������ �ڵ����� ã��
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            //GameObject p = GameObject.FindGameObjectWithTag("Player");
            //if (p) player = p.transform;
        }
    }

   
    public void Interaction()
    {
        PickUp();
    }

    public void PickUp()    // źâ ����, ������ �����ؼ� ���
    {
        Debug.Log(itemName + "�� �ֿ����ϴ�");

        Shooter shooter = player.GetComponentInChildren<Shooter>();
        if (shooter != null)
        {
            switch (weaponType)
            {
                case WeaponType.Gun:
                    shooter.AddAmmo(ammoAmount);
                    break;
                case WeaponType.Rifle:
                    shooter.AddAmmo(ammoAmount);
                    break;
            }
        }

        Destroy(gameObject);
    }

}


