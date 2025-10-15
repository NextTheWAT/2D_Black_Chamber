using System.Collections;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class MoneyPickupPopup : MonoBehaviour
{
    public static MoneyPickupPopup instance { get; private set; }

    [SerializeField] private float showTime = 0.3f; //몇초 보일지
    [SerializeField] private string suffix = "$"; //표시 단위

    private TextMeshProUGUI txt;
    private CanvasGroup cg;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;

        txt = GetComponent<TextMeshProUGUI>();
        cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        if (txt) txt.text = "";
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public void Show(int amount)
    {
        if (amount <= 0 || txt == null || cg == null) return;
        txt.text = $"+{amount}{suffix}";
        StopAllCoroutines();
        StartCoroutine(CoShow());
    }

    private IEnumerator CoShow()
    {
        cg.alpha = 1f;
        yield return new WaitForSeconds(showTime);
        cg.alpha = 0f;
    }
}
