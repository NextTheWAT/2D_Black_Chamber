using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CrosshairUI : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private bool followMouse = true;     // 마우스 따라다님
    [SerializeField] private bool hideHardwareCursor = true;

    [Header("Pulse")]
    [SerializeField] private float shrinkScale = 0.8f;    // 눌렀을 때 축소 비율
    [SerializeField] private float shrinkTime = 0.06f;
    [SerializeField] private float expandTime = 0.12f;
    [SerializeField] private Ease shrinkEase = Ease.OutQuad;
    [SerializeField] private Ease expandEase = Ease.OutBack;

    [Header("CrossHair_Image")]
    [SerializeField] private Image crossHairImage;
    [SerializeField] private RectTransform crossHair_rt;

    private Vector3 baseScale;
    private Sequence seq;

    private void Awake()
    {
        baseScale = crossHair_rt.localScale;
        Cursor.lockState = CursorLockMode.Confined;
        if (hideHardwareCursor) Cursor.visible = false;
    }

    //private void OnDisable()
    //{
    //    seq?.Kill();
    //    if (hideHardwareCursor) Cursor.visible = true;
    //}

    private void Update()
    {
        if (!followMouse || Mouse.current == null) return;
        crossHair_rt.position = Mouse.current.position.ReadValue();   // 화면 좌표 그대로 배치
    }

    public void Pulse()
    {
        seq?.Kill();
        crossHair_rt.localScale = baseScale;
        seq = DOTween.Sequence()
            .Append(crossHair_rt.DOScale(baseScale * shrinkScale, shrinkTime).SetEase(shrinkEase))
            .Append(crossHair_rt.DOScale(baseScale, expandTime).SetEase(expandEase));
    }
}
