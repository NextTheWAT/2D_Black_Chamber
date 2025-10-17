using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelDoor : MonoBehaviour, Iinteraction
{
    public Transform steelDoor;
    public float openDistance;
    private Vector2 closeTransform;
    private Vector2 openTransform;

    public float openSpeed = 0.5f;
    private bool isOpen = false;

    private void Start()
    {
        closeTransform = steelDoor.localPosition;
        openTransform = closeTransform + (Vector2)transform.TransformDirection(new Vector2(0, openDistance));
    }

    private void Update()
    {
        Vector2 door = isOpen ? openTransform : closeTransform;

        steelDoor.localPosition = Vector2.Lerp(steelDoor.localPosition, door, openSpeed * Time.deltaTime);
    }

    public void Interaction(Transform interactor)
    {
        if (!CardKey.hasCardKey) return;
        isOpen = !isOpen;

        if (isOpen)
            StructSoundManager.Instance.PlaySteelDoorOpenSound();
        else
            StructSoundManager.Instance.PlaySteelDoorCloseSound();
    }

}
