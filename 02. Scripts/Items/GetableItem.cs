using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private int quantity = 1;

    public ItemSO ItemSO => itemSO;

    public int         ObjectIndex   => -1;
    public Outline     Outline       { get; private set; }
    public IVisibility VisibilityObj { get; private set; }


    private void Awake()
    {
        Outline = GetComponent<Outline>();
        VisibilityObj = GetComponent<IVisibility>();
        Outline.enabled = false;
    }

    public void Execute()
    {
        bool success = InventoryManager.Instance != null && itemSO != null;
        if (success)
        {
            if (VisibilityObj != null)
            {
                if (!VisibilityObj.IsVisible())
                    return;
            }

            InventoryManager.Instance.AddItem(itemSO, quantity);
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }

    public void Exit()
    {
        Outline.enabled = false;
    }
}