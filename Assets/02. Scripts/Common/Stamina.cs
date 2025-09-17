using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField] private int maxStamina = 100;

    public float currentStamina;

    [SerializeField] private int costPerSecond = 10;    // 소모량
    [SerializeField] private int regenStamina = 5;
    [SerializeField] private float regenDelay = 1.5f;   // 회복딜레이

    private bool isRunning;
    private float lastUseTime;


    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (isRunning)
        {
            Consumption();
        }
        else
        {
            Recovery();
        }
    }

    public void Running()
    {
        if (currentStamina > 0)
            isRunning = true;
        else
            isRunning = false;
    }

    public void StopRunning()
    {
        isRunning = false;
    }

    private void Consumption()
    {
        if (currentStamina > 0)
        {
            //스테미너 소모
            currentStamina -= costPerSecond * Time.deltaTime;

            // 스테미너 소모 마지막 시간
            lastUseTime = Time.time;
        }

        if (currentStamina <= 0)
        {
            currentStamina = 0;
            isRunning = false;
        }
    }

    private void Recovery()
    {
        // 마지막 사용후 딜레이가 끝나면
        if (Time.time >= lastUseTime + regenDelay && currentStamina < maxStamina)
        {
            // 시간당 조금씩 스테미너 회복
            currentStamina += regenStamina * Time.deltaTime;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }
    }
}
