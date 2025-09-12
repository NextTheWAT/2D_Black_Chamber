using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MuzzleFlash : MonoBehaviour
{
    [Header("랜덤 선택할 스프라이트들")]
    [SerializeField] private Sprite[] sprites;

    private void OnEnable()
    {
        if (sprites != null && sprites.Length > 0)
        {
            var sr = GetComponent<SpriteRenderer>();
            int idx = Random.Range(0, sprites.Length);
            sr.sprite = sprites[idx];
        }
        // 0.05초 후에 파괴
        Destroy(gameObject, 0.05f);
    }


}
