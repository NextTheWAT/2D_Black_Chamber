using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    public bool Visible { get; private set; }

    void Update()
    {
        Vector2 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        Visible = viewPos.x >= 0f && viewPos.x <= 1f &&
                        viewPos.y >= 0f && viewPos.y <= 1f;
    }

}
