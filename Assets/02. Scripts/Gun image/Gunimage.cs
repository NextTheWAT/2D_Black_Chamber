using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gunimage : MonoBehaviour
{
    public Sprite[] sprites;
    private Image image;
    private int currentIndex = 0;

    void Awake()
    {
        image = GetComponent<Image>(); 
        image.sprite = sprites[currentIndex]; // 시작 시 첫 번째 이미지 표시
    }

    public void OnLeftButton()
    {
        currentIndex--; //왼쪽으로 이동

        // 인덱스가 0보다 작아지면 맨 끝으로
        if (currentIndex < 0)
        {
            currentIndex = sprites.Length - 1;
        }

        image.sprite = sprites[currentIndex];
        Debug.Log("현재 이미지: " + currentIndex);
    }
    public void OnRightButton()
    {
        currentIndex++; //오른쪽으로 이동

        // 마지막 이미지 이후면 맨 앞으로
        if (currentIndex >= sprites.Length)
        {
            currentIndex = 0;
        }

        image.sprite = sprites[currentIndex];
        Debug.Log("현재 이미지: " + currentIndex);
    }

}
