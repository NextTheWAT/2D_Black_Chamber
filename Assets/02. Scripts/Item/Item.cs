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

   
    public void Interaction(Transform interactor)
    {
        PickUp();
    }

    public void PickUp() //총알 줍기
    {

        var wm = WeaponManager.Instance;
        var shooter = wm != null ? wm.CurrentWeapon : null;
        if (shooter != null)
        {
            int gained = shooter.AddAmmo(ammoAmount); // 실제 증가량을 바로 받음
            if (gained > 0 && AmmoPickupPopup.Instance != null)
            {
                AmmoPickupPopup.Instance.Show(gained);
            }
        }
        Destroy(gameObject);
    }

}


