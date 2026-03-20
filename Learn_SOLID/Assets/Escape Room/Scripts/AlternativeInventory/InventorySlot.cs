using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Image innerPanelIcon;

    public InventoryItemData inventoryItem;
    public InventoryItem inventoryDrag;

    public bool hasItem;

    public void SetItem(InventoryItemData newItem)
    {
        inventoryItem = newItem;
        hasItem = true;

        icon.sprite = newItem.icon;

        SetupItem(newItem);
        AnimateItem();
    }

    void SetupItem(InventoryItemData item)
    {
        bool hasInner = item.hasInnerPanel;

        innerPanelIcon.gameObject.SetActive(hasInner);

        inventoryDrag.canDrag = !hasInner;
    }

    void AnimateItem()
    {
        if (inventoryDrag == null) return;

        RectTransform rect = inventoryDrag.rectTransform;

        rect.SetParent(transform, true);

        rect.DOAnchorPos(Vector2.zero, 0.2f)
            .SetEase(Ease.OutCubic);
    }

    public void ClearSlot()
    {
        hasItem = false;
        inventoryItem = null;

        icon.sprite = null;

        inventoryDrag.ResetSlot();
        inventoryDrag.canDrag = false;

        innerPanelIcon.gameObject.SetActive(false);
    }

    public InventoryItemData GetItem()
    {
        return inventoryItem;
    }
}