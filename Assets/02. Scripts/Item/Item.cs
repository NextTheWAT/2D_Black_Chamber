using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, Iinteraction
{
    public enum WeaponType  // 무기 타입
    {
        Gun,
        Rifle
    }

    public string itemName;
    public WeaponType weaponType; // 권총/라이플 구분
    public int ammoAmount;

    private GameObject player;    // 플레이어 Transform
    //private bool playerInRange = false; // 플레이어가 근처에 있는지 여부
    void Start()
    {
        // Inspector에 player를 안 넣으면 씬에서 자동으로 찾기
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

    public void PickUp()    // 탄창 권총, 라이플 구별해서 드랍
    {
        Debug.Log(itemName + "을 주웠습니다");

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


