using UnityEngine;
using UnityEngine.UI;
// using DG.Tweening;


public abstract class UIBase : MonoBehaviour
{
    public bool IsOpen { get; private set; }
    protected bool Initialized;

    [SerializeField] private RectTransform root;   // 비워두면 자동 할당
    [SerializeField] protected CanvasGroup _canvasGroup;

    protected CanvasGroup canvasGroup => _canvasGroup;

    public virtual void OpenUI()
    {
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);
        gameObject.SetActive(true);
        EnsureRefs();

        IsOpen = true;

        if (_canvasGroup) _canvasGroup.alpha = 1f;

        OnOpen();
    }

    public virtual void CloseUI()
    {
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);
        EnsureRefs();

        // 원래 의미 보존: 닫기 시작 시 훅 호출
        OnClose();

        IsOpen = false;

        gameObject.SetActive(false);
    }

    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }

    void EnsureRefs()
    {
        if (!root) root = transform as RectTransform;
        if (!canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
}
