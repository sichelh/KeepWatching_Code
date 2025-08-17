using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private List<IUIBase> openedUI = new List<IUIBase>();

    public bool IsOpenUI { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    public void CheckOpenPopup(IUIBase panel)
    {
        if (openedUI.Contains(panel))
        {
            panel.Close();
        }
        else
        {
            panel.Open();
        }
    }

    public void OpenPanel(IUIBase panel)
    {
        openedUI.Add(panel);
        IsOpenUI = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ClosePanel(IUIBase panel)
    {
        openedUI.Remove(panel);
        IsOpenUI = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void AllClosePanel()
    {
        for (int i = openedUI.Count - 1; i >= 0; i--)
        {
            openedUI[i].Close();
        }
    }
}