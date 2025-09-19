using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Iinteraction
{
    public float openAngle = 90f;
    public float openSpeed = 1.0f;
    private bool isOpen = false;
    private bool playerInRange = false; // �÷��̾ �����ȿ� ������ ��ȣ�ۿ��ϰ� �ϱ� ����

    private Quaternion closeRotation;   // ������ ȸ����
    private Quaternion openRotation;

    private void Start()
    {

        closeRotation = transform.rotation;    // ó�� ���������� ȸ���� ����
        openRotation = Quaternion.Euler(0, 0, openAngle) * closeRotation;   // �������� ȸ������ ����

        transform.rotation = closeRotation;
    }

    private void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closeRotation;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, openSpeed * Time.deltaTime * 100);
        // RotateTowards : ���� ȸ�� -> ��ǥ ȸ������ ���
    }

    public void Interaction()
    {
        isOpen = !isOpen;
    }

}
