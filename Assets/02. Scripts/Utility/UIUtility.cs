using UnityEngine.EventSystems;

public static class UIUtility
{
    // UI ��Ұ� ���콺 ������ �Ʒ��� �ִ��� Ȯ��
    public static bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

}
