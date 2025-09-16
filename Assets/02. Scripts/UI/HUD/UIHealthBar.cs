using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHealthBar : MonoBehaviour //�÷��̾� ü�� UI ǥ���ϴ� ������Ʈ
{
    [Header("Target")]
    [SerializeField] private Health target; //ǥ���� ���

    [Header("UI Refs")]
    [SerializeField] private Image fill; //ä������ �̹���

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
            //�ǰ� �ٲ𶧸��� UI ������Ʈ
            target.OnHealthChanged.AddListener(UpdateHealthUI);
            UpdateHealthUI(target.CurrentHealth, target.MaxHealth);
        }
        else
        {
            Debug.LogWarning("Ÿ�� HP�� ��� �ֽ��ϴ�.");
        }
    }

    private void OnDisable()
    {
        if (target != null)
            target.OnHealthChanged.RemoveListener(UpdateHealthUI);
    }

    private void UpdateHealthUI(int current, int max) 
        //Health���� ����, �ִ� ���� �ٲ� UI ����
    {
        float ratio = (max > 0) ? (float)current / max : 0f;

        if (fill != null)
            fill.fillAmount = ratio;
    }
}
