using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour, Iinteraction
{
    public int money;

    public void SetAmount(int value)    // ������������ �ݾ��� �����°� ����
    {
        money = value;
    }

    public void Interaction()
    {
        MoneyManager.Instance.AddMoney(money);
        Destroy(gameObject);
    }
}
