using UnityEngine.EventSystems;

public static class UIUtility
{
    // UI 요소가 마우스 포인터 아래에 있는지 확인
    public static bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

}
