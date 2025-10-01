using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Papermoney: MonoBehaviour, Iinteraction
{
    public enum MoneyType  // �� Ÿ��
    {
        Coin,
        Papermoney
    }
    public string itemName;
    public MoneyType moneyType;
    private GameObject player;  // �÷��̾� Transform
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

    public void PickUp()
    {
        Debug.Log(itemName + "�� �ֿ����ϴ�");

        Destroy(gameObject);
    }


}
