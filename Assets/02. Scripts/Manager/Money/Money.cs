using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour, Iinteraction
{
    public int money = 10;

    public void Interaction()
    {
        MoneyManager.Instance.AddMoney(money);
        Destroy(gameObject);
    }
}
