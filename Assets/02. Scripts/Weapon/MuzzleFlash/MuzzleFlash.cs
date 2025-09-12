using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MuzzleFlash : MonoBehaviour
{
    [Header("���� ������ ��������Ʈ��")]
    [SerializeField] private Sprite[] sprites;

    private void OnEnable()
    {
        if (sprites != null && sprites.Length > 0)
        {
            var sr = GetComponent<SpriteRenderer>();
            int idx = Random.Range(0, sprites.Length);
            sr.sprite = sprites[idx];
        }
        // 0.05�� �Ŀ� �ı�
        Destroy(gameObject, 0.05f);
    }


}
