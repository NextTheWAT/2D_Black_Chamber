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
            Vector2 doorPosition = new Vector2(transform.position.x, transform.position.y);    // �� ��ġ
            Vector2 playerPosition = new Vector2(player.position.x, player.position.y);        // �÷��̾� ��ġ
            Vector2 direction = (playerPosition - doorPosition);

            Vector2 localDirection = transform.InverseTransformPoint(player.position);

            float angle = (localDirection.x >= 0) ? -90f : 90f;

            targetRotation = closeRotation * Quaternion.Euler(0, 0, angle);
        }
        else
        {
            targetRotation = closeRotation;
        }

        isOpen = !isOpen;
    }

}
