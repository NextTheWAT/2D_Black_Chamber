using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public bool IsOpen { get; private set; }
    protected bool Initialized;


    public virtual void OpenUI()
    {
        gameObject.SetActive(true);
        IsOpen = true;
        OnOpen();
    }

    public virtual void CloseUI()
    {
        OnClose();
        IsOpen = false;
        gameObject.SetActive(false);
    }

    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }
}
