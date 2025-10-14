using System.Collections;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class AmmoPickupPopup : MonoBehaviour
{
    public static AmmoPickupPopup Instance { get; private set; }

    private TextMeshProUGUI popupText;
    private CanvasGroup cg;
    [SerializeField] private float showTime = 1f; //Ç¥½Ã ½Ã°£

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        popupText = GetComponent<TextMeshProUGUI>();
        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        popupText.text = "";
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Show (int amount) //Åº¾à È¹µæ ½Ã +n Ç¥½Ã
    {
        if (amount <= 0) return;
        if (popupText == null || cg == null) return;

        popupText.text = $"+{amount}";
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        cg.alpha = 1f;
        yield return new WaitForSeconds(showTime);
        cg.alpha = 0f;
    }
}
