using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trigger_OpenPopup2D : MonoBehaviour
{
    [Header("팝업 프리팹")]
    [SerializeField] private GameObject[] popupPrefabs;

    [Header("팝업 프리팹 선택")]
    [SerializeField, Tooltip("몇번째 팝업을 띄울지")] private int popupIndex = 0;

    [Header("한번만 작동할지 여부")]
    [SerializeField] private bool oneShot = true;
    
    private bool opened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (opened && oneShot) return;

        if (popupPrefabs == null || popupPrefabs.Length == 0)
        {
            Debug.LogWarning("[Trigger_OpenPopup2D] popupPrefabs 배열이 비어있습니다.");
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
