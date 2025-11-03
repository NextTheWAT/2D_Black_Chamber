using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyItem : MonoBehaviour, Iinteraction
{
    public int money;

    public void SetAmount(int value)    // 생성된프리펩 금액을 가져온걸 저장
    {
        money = value;
    }

    public void Interaction(Transform interactor)
    {
        // 1. 플레이어의 총 잔액에 돈을 추가 (MoneyManager)
        MoneyManager.Instance.Add(money);

        // 2. 클리어 통계를 위해 GameStats에도 돈을 기록
        if (GameStats.Instance != null)
        {
            GameStats.Instance.AddMoney(money);
        }

        // 3. 팝업 표시
        if (MoneyPickupPopup.instance != null)
            MoneyPickupPopup.instance.Show(money);

        // 4. 사운드 재생
        StructSoundManager.Instance.PlayPickUpSound(transform.position);

        // 5. 아이템 파괴
        Destroy(gameObject);
    }
}
