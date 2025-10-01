using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : UIBase
{
    [Header("Refs")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button dimmerButton;

    [Header("Anim")]
    [SerializeField] private float fadeDuration = 0.18f;

    private Coroutine _fadeCo;

    private void Start()
    {
        gameObject.SetActive(false);
    }


    private void Reset()
    {
        if (canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        if (closeButton == null) closeButton = GetComponentInChildren<Button>(true);
        if (dimmerButton == null)
        {
            var dimmer = transform.Find("Dimmer");
            if (dimmer) dimmerButton = dimmer.GetComponent<Button>();
        }
    }

    protected override void OnOpen()
    {
        if (!Initialized)
        {
            if (closeButton) closeButton.onClick.AddListener(RequestClose);
            if (dimmerButton) dimmerButton.onClick.AddListener(RequestClose);
            Initialized = true;
        }

        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            if (_fadeCo != null) StopCoroutine(_fadeCo);
            _fadeCo = StartCoroutine(Fade(0f, 1f, fadeDuration));
        }
    }

    public void RequestClose()
    {
        if (!gameObject.activeInHierarchy) return;
        if (_fadeCo != null) StopCoroutine(_fadeCo);
        _fadeCo = StartCoroutine(CloseSequence());
    }

    private IEnumerator CloseSequence()
    {
        if (canvasGroup)
        {
            canvasGroup.interactable = false;
            // 알파 1 > 0 페이드 아웃
            yield return Fade(1f, 0f, fadeDuration);
            canvasGroup.blocksRaycasts = false;
        }
        UIManager.Instance.CloseUI<SettingPopup>();
    }

    protected override void OnClose()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator Fade(float from, float to, float dur)
    {
        float t = 0f;
        if (dur <= 0f)
        {
            if (canvasGroup) canvasGroup.alpha = to;
            yield break;
        }

        if (canvasGroup) canvasGroup.alpha = from;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / dur);
            if (canvasGroup) canvasGroup.alpha = a;
            yield return null;
        }
        if (canvasGroup) canvasGroup.alpha = to;
    }
}
