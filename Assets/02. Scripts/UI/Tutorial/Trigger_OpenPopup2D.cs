using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trigger_OpenPopup2D : MonoBehaviour
{
    [Header("�˾� ������")]
    [SerializeField] private GameObject[] popupPrefabs;

    [Header("�˾� ������ ����")]
    [SerializeField, Tooltip("���° �˾��� �����")] private int popupIndex = 0;

    [Header("�ѹ��� �۵����� ����")]
    [SerializeField] private bool oneShot = true;
    
    private bool opened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (opened && oneShot) return;

        if (popupPrefabs == null || popupPrefabs.Length == 0)
        {
            Debug.LogWarning("[Trigger_OpenPopup2D] popupPrefabs �迭�� ����ֽ��ϴ�.");
            return;
        }

        int safeIndex = Mathf.Clamp(popupIndex, 0, popupPrefabs.Length - 1);
        var prefab = popupPrefabs[safeIndex];

        Instantiate(prefab);
        opened = true;
    }

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }
}
