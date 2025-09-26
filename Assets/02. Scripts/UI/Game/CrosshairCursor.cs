using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairCursor : MonoBehaviour
{
    [Header("크로스헤어")]
    [SerializeField] private Canvas canvas;

    private RectTransform rect;
    private RectTransform canvasRect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.transform as RectTransform;
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        Vector2 screenPos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector2)Input.mousePosition;

        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out var local))
        {
            rect.anchoredPosition = local;
        }
    }
}
