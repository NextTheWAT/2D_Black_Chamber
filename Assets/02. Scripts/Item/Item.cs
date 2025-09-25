using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public string itemName;

    public void PickUp()
    {
        Debug.Log(itemName + "을(를) 주웠습니다!");
        Destroy(gameObject); // 아이템 삭제
    }

}




