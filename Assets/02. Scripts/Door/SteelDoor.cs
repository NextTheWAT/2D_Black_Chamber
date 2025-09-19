using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelDoor : MonoBehaviour
{
    public Transform steelDoor;
    public float openDistance;
    private Vector2 closeTransform;
    private Vector2 openTransform;

    public float openSpeed = 0.5f;
    private bool isOpen = false;
    private bool playerInRange = false;

    private void Start()
    {
        closeTransform = steelDoor.localPosition;
        openTransform = closeTransform + new Vector2(0, openDistance);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            isOpen = !isOpen;

        Vector2 door = isOpen ? openTransform : closeTransform;

        steelDoor.localPosition = Vector2.Lerp(steelDoor.localPosition, door, openSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }
}
