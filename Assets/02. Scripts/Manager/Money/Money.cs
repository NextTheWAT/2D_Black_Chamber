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

    public void Interaction(Transform interactor)
    {
        MoneyManager.Instance.AddMoney(money);

        if (MoneyPickupPopup.instance != null )
            MoneyPickupPopup.instance.Show(money);

        StructSoundManager.Instance.PlayPickUpSound(transform.position);

        Destroy(gameObject);
    }
}
