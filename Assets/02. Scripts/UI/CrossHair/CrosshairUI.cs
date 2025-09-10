using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class CrosshairUI : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private bool followMouse = true;     // ���콺 ����ٴ�
    [SerializeField] private bool hideHardwareCursor = true;

    [Header("Pulse")]
    [SerializeField] private float shrinkScale = 0.8f;    // ������ �� ��� ����
    [SerializeField] private float shrinkTime = 0.06f;
    [SerializeField] private float expandTime = 0.12f;
    [SerializeField] private Ease shrinkEase = Ease.OutQuad;
    [SerializeField] private Ease expandEase = Ease.OutBack;

    private RectTransform rt;
    private Vector3 baseScale;
    private Sequence seq;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        baseScale = rt.localScale;
        Cursor.lockState = CursorLockMode.Confined;
        if (hideHardwareCursor) Cursor.visible = false;
    }

    private void OnDisable()
    {
        seq?.Kill();
        if (hideHardwareCursor) Cursor.visible = true;
    }

    private void Update()
    {
        if (!followMouse || Mouse.current == null) return;
        rt.position = Mouse.current.position.ReadValue();   // ȭ�� ��ǥ �״�� ��ġ
    }

    public void Pulse()
    {
        seq?.Kill();
        rt.localScale = baseScale;
        seq = DOTween.Sequence()
            .Append(rt.DOScale(baseScale * shrinkScale, shrinkTime).SetEase(shrinkEase))
            .Append(rt.DOScale(baseScale, expandTime).SetEase(expandEase));
    }
}
