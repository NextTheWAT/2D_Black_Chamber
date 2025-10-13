using UnityEngine;

public class Key_Anim : MonoBehaviour
{
    [Header("���� ���")]
    [SerializeField] private Transform target;

    [Header("���� ��ǥ ���� ������")]
    [SerializeField] private float worldOffsetX = 0.7f; // �÷��̾� ��ġ.x + �� ��
    [SerializeField] private float worldOffsetY = 0.4f; // �÷��̾� ��ġ.y + �� ��
    [SerializeField] private float zWorld = 0f;         // 2D�� 0 ���� ����

    [Header("�θ� ���� ���� �ɼ�")]
    [SerializeField] private bool keepUpright = true;     // �׻� ȸ�� 0��(�÷��̾� ȸ�� ����)

    private void Awake()
    {
        if (!target)
        {
            // �θ� ü�ο��� �ڵ����� �÷��̾� ã��(������ �׳� �θ� ���)
            var pic = GetComponentInParent<PlayerInputController>();
            target = pic ? pic.transform : transform.parent;
        }
    }

    void Start()
    {
        //gameObject.SetActive(false); // ������ ���� �����
    }

    private void LateUpdate()
    {
        if (!target) return;

        // 1) ���� ��ġ�� "�÷��̾� ��ġ + (X,Y) ������"���� ��ġ
        Vector3 tp = target.position;
        transform.position = new Vector3(tp.x + worldOffsetX, tp.y + worldOffsetY, zWorld);

        // 2) ȸ�� ����: �÷��̾ ���Ƶ� ������Ʈ�� �׻� �ȹٷ�
        if (keepUpright)
            transform.rotation = Quaternion.identity;
    }

    // ��Ÿ�ӿ��� ������ �ٲٰ� ���� �� ȣ���
    public void SetOffset(float x, float y)
    {
        worldOffsetX = x;
        worldOffsetY = y;
    }
}
