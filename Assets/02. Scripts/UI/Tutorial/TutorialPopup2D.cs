using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class TutorialPopup2D : MonoBehaviour
{
    [Header("����")]
    [SerializeField] private SpriteRenderer imageRenderer;
    [SerializeField] private SpriteRenderer dimBackground;

    [Header("������ ����Ʈ")]
    [SerializeField] private List<Sprite> pages = new();

    [Header("������ ����")]
    [SerializeField] private string loadSceneAfterLastPage = ""; //����θ� �ݱ⸸��

    public System.Action onClosed;

    int index = 0;
    bool isOpen = false;

    private void Awake()
    {
        if (imageRenderer == null)
            Debug.LogWarning("[TutorialPopup2D] imageRenderer�� ����ֽ��ϴ�.");
        if (pages.Count == 0)
            Debug.LogWarning("[TutorialPopup2D] pages�� ����ֽ��ϴ�.");

        Open();
    }

    private void Update()
    {
        if (!isOpen || Keyboard.current == null) return;
        if (Keyboard.current.fKey.wasPressedThisFrame) //FŰ�� ������
            Next();
    }

    void Open()
    {
        isOpen = true;
        SetPage(0);
        if (dimBackground != null) dimBackground.enabled = true;
    }

    void SetPage(int i)
    {
        index = Mathf.Clamp(i, 0, Mathf.Max(0, pages.Count - 1));
        if (imageRenderer != null && pages.Count > 0)
            imageRenderer.sprite = pages[index];
    }

    void Next()
    {
        if (index < pages.Count - 1)
        {
            SetPage(index + 1);
            return;
        }

        if (!string.IsNullOrEmpty(loadSceneAfterLastPage))
            SceneManager.LoadScene(loadSceneAfterLastPage);
        else
            Close();
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        if (dimBackground != null) dimBackground.enabled = false;
        onClosed?.Invoke();
        Destroy(gameObject);
    }
}
