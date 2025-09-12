using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    public float baseIntensity = 1f;        // ±âº» ¹à±â
    public float flickerAmount = 0.4f;      // ±ôºýÀÓ Á¤µµ
    public float flickerSpeed = 2f;         // ±ôºýÀÓ ºü¸£±â

    private Light2D light2D;


    private void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    private void Update()
    {
        float instensity = baseIntensity + Mathf.Sin(Time.time * flickerSpeed) * flickerAmount;
        light2D.intensity = Mathf.Max(0, instensity);
    }
}
