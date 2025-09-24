using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    private int money;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddMoney(int amount)
    {
        // ÇÁ¸®Æé º°·Î ´Ù¸£°Ô µ· È¹µæ?
        money += amount;
    }

    public void SpendMoney(int amount)
    {
        // µ· »ç¿ë
        if (money >= amount)
        {
            money -= amount;
            return;
        }
        else
            return;
    }

    public int GetMoney()
    {
        return money;
    }
}
