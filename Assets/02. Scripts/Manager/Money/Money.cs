using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour, Iinteraction
{
    public int money;

    public void SetAmount(int value)    // 생성된프리펩 금액을 가져온걸 저장
    {
        money = value;
    }

    public void Interaction()
    {
        MoneyManager.Instance.AddMoney(money);
        Destroy(gameObject);
    }
}
