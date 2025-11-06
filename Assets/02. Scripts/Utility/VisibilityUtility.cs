using UnityEngine;

public static class VisibilityUtility
{
    public static bool IsVisible(Vector2 pos)
    {
        Vector2 viewPos = Camera.main.WorldToViewportPoint(pos);

        return viewPos.x >= 0f && viewPos.x <= 1f &&
                        viewPos.y >= 0f && viewPos.y <= 1f;
    }
}
