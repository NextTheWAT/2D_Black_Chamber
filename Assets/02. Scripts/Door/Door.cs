using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Iinteraction
{
    public bool isExitDoor = false;

    //public float openAngle = 90f;
    public float openSpeed = 1.0f;
    private bool isOpen = false;
    //private bool playerInRange = false; // �÷��̾ �����ȿ� ������ ��ȣ�ۿ��ϰ� �ϱ� ����

    public bool IsOpen => isOpen;

    private Quaternion closeRotation;   // ������ ȸ����
    private Quaternion targetRotation;

    private void Start()
    {
        closeRotation = transform.rotation;    // ó�� ���������� ȸ���� ����
        targetRotation = closeRotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, openSpeed * Time.deltaTime * 100);
        // RotateTowards : ���� ȸ�� -> ��ǥ ȸ������ ���
    }

    public void Interaction(Transform interactor)
    {
        if (isExitDoor) return;
        if (!interactor) return;
        
        if (!isOpen)
        {
            Vector2 doorPosition = new Vector2(transform.position.x, transform.position.y);    // �� ��ġ
            Vector2 interactorPosition = new Vector2(interactor.position.x, interactor.position.y);        // ��ȣ�ۿ��� ��ġ
            Vector2 direction = (interactorPosition - doorPosition);

            Vector2 localDirection = transform.InverseTransformPoint(interactor.position);  // ������ǥ�� ������ǥ�� �ٲ���

            float angle = (localDirection.x >= 0) ? -90f : 90f;

            targetRotation = closeRotation * Quaternion.Euler(0, 0, angle);
        }
        else
        {
            targetRotation = closeRotation;
        }

        isOpen = !isOpen;
    }

    public void AutoOpen()
    {
        Debug.Log("Ż�ⱸ�� ����");
        if (!isExitDoor)
            return;

        if (isOpen) return;

        gameObject.SetActive(false);
        isOpen = true;
    }

}
