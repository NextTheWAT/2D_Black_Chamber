using UnityEngine;
using UnityEngine.UI;

public class SettingsOverlay : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private Button closeButton; // Window/Header/CloseButton

    private bool isOpen;

    private void Reset()
    {
        if (!overlayRoot) overlayRoot = gameObject;
        if (!closeButton)
            closeButton = transform.Find("Window/Header/CloseButton")?.GetComponent<Button>();
    }

    private void Awake()
    {
        if (!overlayRoot) overlayRoot = gameObject;
        overlayRoot.SetActive(false);
        isOpen = false;

        if (closeButton) closeButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        // 같은 Canvas 안에서 항상 최상단으로
        transform.SetAsLastSibling();
        overlayRoot.SetActive(true);
        isOpen = true;
    }

    public void Close()
    {
        overlayRoot.SetActive(false);
        isOpen = false;
    }

    public void Toggle()
    {
        if (isOpen) Close(); else Open();
    }
}
