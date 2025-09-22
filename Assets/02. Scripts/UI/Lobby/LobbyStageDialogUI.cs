using UnityEngine;
using UnityEngine.UI;

public class LobbyStageDialogUI : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private Button closeButton;

    private bool isOpen = false;
    private float prevTimeScale = 1f;
    private bool prevCursorVisible;
    private CursorLockMode prevCursorLock;

    private void Awake()
    {
        if (overlayRoot == null) overlayRoot = gameObject;
        overlayRoot.SetActive(false);
        if (closeButton) closeButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        //���� ���, �Ͻ�����
        prevTimeScale = Time.timeScale;
        prevCursorVisible = Cursor.visible;
        prevCursorLock = Cursor.lockState;

        Time.timeScale = 0f;

        //UI ǥ�� + Ŀ�� Ȱ��
        overlayRoot.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        overlayRoot.SetActive(false);

        // �ð�/Ŀ�� ����
        Time.timeScale = Mathf.Approximately(prevTimeScale, 0f) ? 1f : prevTimeScale;
        Cursor.visible = prevCursorVisible;
        Cursor.lockState = prevCursorLock;
    }
}
