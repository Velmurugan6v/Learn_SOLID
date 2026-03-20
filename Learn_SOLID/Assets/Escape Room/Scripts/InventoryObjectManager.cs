using System.Collections.Generic;
using UnityEngine;

public class InventoryObjectManager : MonoBehaviour
{
    public static InventoryObjectManager instance;

    public List<InventoryItemData> inventoryItemDatas;
    public List<InventorySlot> inventorySlots;
    
    public AudioManager audioManager;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitInventoryObjects();
    }

    void InitInventoryObjects()
    {
        for (int i = 0; i < inventoryItemDatas.Count; i++)
        {
            int index = i;

            inventoryItemDatas[i].collectButton.onClick
                .AddListener(() => AddItemToFront(inventoryItemDatas[index]));
        }

        /*HintGraphRunner.instance.SetItemCollected(id);
        HintGraphRunner.instance.SetFlag("key");*/
    }

    public void AddItemToFront(InventoryItemData newItem)
    {
        audioManager.PlaySound(0);
        
        ShiftRight();

        inventorySlots[0].SetItem(newItem);

        newItem.interactionState.SetInteractionState();

        newItem.onCollected?.Invoke();
        //UpdateDragState();
    }

    void ShiftRight()
    {
        /*for (int i = inventorySlots.Length - 1; i > 0; i--)
        {
            inventorySlots[i].SetItem(inventorySlots[i - 1].inventoryItem);
        }*/

        for (int i = inventorySlots.Count - 1; i > 0; i--)
        {
            var sourceSlot = inventorySlots[i - 1];
            var destinationSlot = inventorySlots[i];

            if (sourceSlot.hasItem)
                destinationSlot.SetItem(sourceSlot.inventoryItem);
            else
                destinationSlot.ClearSlot();
        }
    }

    public void RemoveItem(InventoryItemData item)
    {
        int index = FindItemIndex(item);

        if (index == -1) return;

        ShiftLeft(index);

        inventorySlots[inventorySlots.Count - 1].ClearSlot();
    }

    int FindItemIndex(InventoryItemData item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].inventoryItem == item)
                return i;
        }

        return -1;
    }

    void ShiftLeft(int startIndex)
    {
        /*for (int i = startIndex; i < inventorySlots.Length - 1; i++)
        {
            inventorySlots[i].SetItem(inventorySlots[i + 1].inventoryItem);
        }*/

        for (int i = startIndex; i < inventorySlots.Count - 1; i++)
        {
            InventorySlot slot = inventorySlots[i + 1];
            InventoryItemData item = inventorySlots[i + 1].inventoryItem;

            if (slot.hasItem)
                inventorySlots[i].SetItem(item);
            else
                inventorySlots[i].ClearSlot();
        }
    }

    void UpdateDragState()
    {
        foreach (var slot in inventorySlots)
        {
            if (!slot.hasItem)
                slot.inventoryDrag.canDrag = false;
        }
    }

    public void RemoveInventoryItem(int item)
    {
        InventoryItemData itemData = inventoryItemDatas[item];

        RemoveItem(itemData);
    }
}