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

    public void Interaction(Transform interactor)
    {
        MoneyManager.Instance.AddMoney(money);

        if (MoneyPickupPopup.instance != null )
            MoneyPickupPopup.instance.Show(money);

        StructSoundManager.Instance.PlayPickUpSound(transform.position);

        Destroy(gameObject);
    }
}
