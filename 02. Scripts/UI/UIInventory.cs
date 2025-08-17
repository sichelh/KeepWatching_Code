using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventory : UIBase<UIInventory>, IUIBase
{
    [SerializeField] private InventorySlot[] inventorySlots;

    [Header("ItemInfo")]
    [SerializeField] private TextMeshProUGUI itemName;

    [SerializeField] private TextMeshProUGUI itemDescription;

    public InventorySlot SelectedItem { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InitializeSlots();
        InventoryManager.Instance.OnInventorySlotUpdate += UpdateInventorySlot;
    }

    private void InitializeSlots()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SetItem(i, InventoryManager.Instance.Inventory[i]);
        }
    }

    public void SelecteItem(InventorySlot item)
    {
        if (SelectedItem != null && SelectedItem != item)
            SelectedItem.DeSelectedSlot();

        SelectedItem = item;
        ShowItemInfo();
    }

    private void UpdateInventorySlot(int index)
    {
        if (index < 0 || index >= inventorySlots.Length)
            return;

        InventoryItem itemData = InventoryManager.Instance.Inventory[index];

        inventorySlots[index].SetItem(index, itemData);
    }

    private void ShowItemInfo()
    {
        if (SelectedItem == null)
            return;
        itemName.text = SelectedItem.InventoryItem.ItemSo.ItemName;
        itemDescription.text = SelectedItem.InventoryItem.ItemSo.ItemDescription;
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        SelectedItem?.DeSelectedSlot();
        SelectedItem = null;
        base.Close();
    }
}