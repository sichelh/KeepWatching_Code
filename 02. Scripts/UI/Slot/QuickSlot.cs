using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemQtyText;

    public int RegistedInventoryIndex { get; private set; } = -1;

    public InventoryItem RegistedItem { get; private set; }

    public void SetItemSlot(int index)
    {
        RegistedItem = InventoryManager.Instance.GetInventoryItemAtSlot(index);
        if (RegistedItem == null || RegistedItem.Quantity == 0)
        {
            EmptySlot();
            return;
        }

        RegisteredItemByIndex(index);
    }

    private void EmptySlot()
    {
        icon.enabled = false;
        RegistedItem = null;
        itemQtyText.text = "";
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!DragManager.Instance.IsDragging || DragManager.Instance.DraggedInventoryItem == null
                                             || UIHUD.Instance.QuickSlots.Find(x => x.RegistedItem == DragManager.Instance.DraggedInventoryItem.InventoryItem))
            return;

        InventorySlot draggedItem = DragManager.Instance.DraggedInventoryItem;

        SetItemSlot(draggedItem.Index);
    }

    private void RegisteredItemByIndex(int index)
    {
        icon.enabled = true;
        icon.sprite = RegistedItem.ItemSo.ItemSprite;
        itemQtyText.text = RegistedItem.Quantity > 1 ? $"{RegistedItem.Quantity}" : "";
        RegistedInventoryIndex = index;
        InventoryManager.Instance.OnInventorySlotUpdate += UpdateHUDSlot;
    }

    private void UpdateHUDSlot(int index)
    {
        if (index != RegistedInventoryIndex || RegistedItem == null)
            return;
        if (RegistedItem.Quantity == 0)
        {
            EmptySlot();
            return;
        }

        itemQtyText.text = RegistedItem.Quantity.ToString();
    }
}