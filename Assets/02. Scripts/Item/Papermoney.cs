using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Papermoney: MonoBehaviour, Iinteraction
{
    public enum MoneyType  // 돈 타입
    {
        Coin,
        Papermoney
    }
    public string itemName;
    public MoneyType moneyType;
    private GameObject player;  // 플레이어 Transform
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

    public void PickUp()
    {
        Debug.Log(itemName + "을 주웠습니다");

        Destroy(gameObject);
    }


}
