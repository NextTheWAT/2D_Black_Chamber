using UnityEngine;

// ���� ���۵� ��, �� ������ ������ �� UIKey�� UIRoot�� �˷��ִ� �ʱ�ȭ ��ũ��Ʈ.
public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private UIRoot uiRoot;   // UIRoot ������ ����
    [SerializeField] private UIKey activeUI;  // �� ������ ������ Canvas ����

    private void Start()
    {
        if (uiRoot != null)
            uiRoot.ShowOnly(activeUI);
        else
            Debug.LogError("[SceneInitializer] uiRoot ������ ����");

        //�߰�: �� ���ؽ�Ʈ�� BGMManager�� ����
        BGMManager.Instance?.SetUiContext(activeUI, instant: true);
    }
}
