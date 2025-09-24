using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Iinteraction
{
    //public float openAngle = 90f;
    public float openSpeed = 1.0f;
    private bool isOpen = false;
    //private bool playerInRange = false; // �÷��̾ �����ȿ� ������ ��ȣ�ۿ��ϰ� �ϱ� ����

    private Quaternion closeRotation;   // ������ ȸ����
    private Quaternion targetRotation;

    public Transform player;

    private void Start()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        closeRotation = transform.rotation;    // ó�� ���������� ȸ���� ����
        targetRotation = closeRotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, openSpeed * Time.deltaTime * 100);
        // RotateTowards : ���� ȸ�� -> ��ǥ ȸ������ ���
    }

    public void Interaction()
    {
        if (!isOpen)
        {
            Vector3 playerPosition = (player.position - transform.position).normalized; // �÷��̾ ��� ���ִ��� �Ǵ�
            float dot = Vector3.Dot(transform.forward, playerPosition); // ���� ������� �÷��̾ ������ ������

            float angle = (dot > 0) ? 90f : -90f;

            Vector3 openPosition = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;

            targetRotation = Quaternion.LookRotation(openPosition, Vector3.up);
        }
        else
        {
            targetRotation = closeRotation;
        }

        isOpen = !isOpen;
    }

}
