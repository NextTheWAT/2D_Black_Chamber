using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    public float baseIntensity = 1f;        // 기본 밝기
    public float flickerAmount = 0.4f;      // 깜빡임 정도
    public float flickerSpeed = 2f;         // 깜빡임 빠르기

    private Light2D light2D;


    private void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    private void Update()
    {
        float instensity = baseIntensity + Mathf.Sin(Time.time * flickerSpeed) * flickerAmount; // Mathf.Sin = 주기적으로 반복되게 하는 함수
        light2D.intensity = Mathf.Max(0, instensity);
    }
}
