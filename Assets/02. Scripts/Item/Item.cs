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

   
    public void Interaction(Transform interactor)
    {
        PickUp();
    }

    public void PickUp() //�Ѿ� �ݱ�
    {

        var wm = WeaponManager.Instance;
        var shooter = wm != null ? wm.CurrentWeapon : null;
        if (shooter != null)
        {
            int gained = shooter.AddAmmo(ammoAmount); // ���� �������� �ٷ� ����
            if (gained > 0 && AmmoPickupPopup.Instance != null)
            {
                AmmoPickupPopup.Instance.Show(gained);
            }
        }
        Destroy(gameObject);
    }

}


