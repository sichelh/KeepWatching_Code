using UnityEngine;

public class UIBase<T> : SceneOnlySingleton<T> where T : UIBase<T>, IUIBase
{
    [Header("UIBase")]
    [SerializeField] private GameObject content;

    public virtual void Open()
    {
        content.SetActive(true);
        UIManager.Instance.AllClosePanel();
        UIManager.Instance.OpenPanel(this as IUIBase);
    }

    public virtual void Close()
    {
        content.SetActive(false);
        UIManager.Instance.ClosePanel(this as IUIBase);
    }
}