using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CrosshairCursor : MonoBehaviour
{
    [Header("크로스헤어")]
    [SerializeField] private Canvas canvas;

    [SerializeField] private Image upBar;
    [SerializeField] private Image downBar;
    [SerializeField] private Image leftBar;
    [SerializeField] private Image rightBar;

    private RectTransform rect;
    private RectTransform canvasRect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.transform as RectTransform;
        foreach (var g in GetComponentsInChildren<Graphic>(true))
            g.raycastTarget = false;
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

        UpdateCrosshairSpread();
    }

    public void UpdateCrosshairSpread()
    {
        if (WeaponManager.Instance.CurrentWeapon == null) return;

        Shooter shooter = WeaponManager.Instance.CurrentWeapon;
        Vector2 gunPoint = Camera.main.WorldToScreenPoint(shooter.gunPoint.position);
        Vector2 mousePos = Input.mousePosition;

        float distance = Vector2.Distance(gunPoint, mousePos);
        float angle = shooter.CurrentSpread * Mathf.Deg2Rad;

        float deltaWorld = distance * Mathf.Tan(angle);
        SetCrosshairDelta(deltaWorld * 0.5f);
    }

    public void SetCrosshairDelta(float delta)
    {
        upBar.rectTransform.anchoredPosition = new Vector2(0, delta);
        downBar.rectTransform.anchoredPosition = new Vector2(0, -delta);
        leftBar.rectTransform.anchoredPosition = new Vector2(-delta, 0);
        rightBar.rectTransform.anchoredPosition = new Vector2(delta, 0);
    }
}
