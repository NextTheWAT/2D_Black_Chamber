using UnityEngine;

public class Key_Anim : MonoBehaviour
{
    [Header("따라갈 대상")]
    [SerializeField] private Transform target;

    [Header("월드 좌표 기준 오프셋")]
    [SerializeField] private float worldOffsetX = 0.7f; // 플레이어 위치.x + 이 값
    [SerializeField] private float worldOffsetY = 0.4f; // 플레이어 위치.y + 이 값
    [SerializeField] private float zWorld = 0f;         // 2D면 0 고정 권장

    [Header("부모 영향 제거 옵션")]
    [SerializeField] private bool keepUpright = true;     // 항상 회전 0도(플레이어 회전 무시)

    private void Awake()
    {
        if (!target)
        {
            // 부모 체인에서 자동으로 플레이어 찾기(없으면 그냥 부모 사용)
            var pic = GetComponentInParent<PlayerInputController>();
            target = pic ? pic.transform : transform.parent;
        }
    }

    void Start()
    {
        //gameObject.SetActive(false); // 시작할 때는 숨기기
    }

    private void LateUpdate()
    {
        if (!target) return;

        // 1) 월드 위치를 "플레이어 위치 + (X,Y) 고정값"으로 배치
        Vector3 tp = target.position;
        transform.position = new Vector3(tp.x + worldOffsetX, tp.y + worldOffsetY, zWorld);

        // 2) 회전 고정: 플레이어가 돌아도 프롬프트는 항상 똑바로
        if (keepUpright)
            transform.rotation = Quaternion.identity;
    }

    // 런타임에서 오프셋 바꾸고 싶을 때 호출용
    public void SetOffset(float x, float y)
    {
        worldOffsetX = x;
        worldOffsetY = y;
    }
}
