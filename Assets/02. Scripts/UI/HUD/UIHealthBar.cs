using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHealthBar : MonoBehaviour //플레이어 체력 UI 표시하는 컴포넌트
{
    [Header("Target")]
    [SerializeField] private Health target; //표시할 대상

    [Header("UI Refs")]
    [SerializeField] private Image fill; //채워지는 이미지

    private void Awake()
    {
        if (target == null && GameManager.Instance != null && GameManager.Instance.player != null)
        {
            target = GameManager.Instance.player.GetComponent<Health>();
        }
    }

    private void OnEnable()
    {
        if (target != null)
        {
            //피가 바뀔때마다 UI 업데이트
            target.OnHealthChanged.AddListener(UpdateHealthUI);
            UpdateHealthUI(target.CurrentHealth, target.MaxHealth);
        }
        else
        {
            Debug.LogWarning("타겟 HP가 비어 있습니다.");
        }
    }

    private void OnDisable()
    {
        if (target != null)
            target.OnHealthChanged.RemoveListener(UpdateHealthUI);
    }

    private void UpdateHealthUI(int current, int max) 
        //Health에서 현재, 최대 값이 바뀔때 UI 갱신
    {
        float ratio = (max > 0) ? (float)current / max : 0f;

        if (fill != null)
            fill.fillAmount = ratio;
    }
}
