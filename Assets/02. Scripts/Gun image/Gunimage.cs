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
        image.sprite = sprites[currentIndex]; // ���� �� ù ��° �̹��� ǥ��
    }

    public void OnLeftButton()
    {
        currentIndex--; //�������� �̵�

        // �ε����� 0���� �۾����� �� ������
        if (currentIndex < 0)
        {
            currentIndex = sprites.Length - 1;
        }

        image.sprite = sprites[currentIndex];
        Debug.Log("���� �̹���: " + currentIndex);
    }
    public void OnRightButton()
    {
        currentIndex++; //���������� �̵�

        // ������ �̹��� ���ĸ� �� ������
        if (currentIndex >= sprites.Length)
        {
            currentIndex = 0;
        }

        image.sprite = sprites[currentIndex];
        Debug.Log("���� �̹���: " + currentIndex);
    }

}
