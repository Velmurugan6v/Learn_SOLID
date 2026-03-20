using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour,
    IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler
{
    public Transform initialParent;
    public Transform dragParent;

    public InventorySlot inventorySlot;

    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    public Canvas canvas;

    public bool canDrag;

    public InnerPanelManager innerPanelManager;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        transform.SetParent(dragParent, true);

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        canvasGroup.blocksRaycasts = true;

        transform.SetParent(initialParent, true);

        rectTransform.DOAnchorPos(Vector2.zero, 0.2f);
    }

    public void ResetSlot()
    {
        canvasGroup.blocksRaycasts = true;

        transform.SetParent(initialParent, true);

        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventorySlot == null || inventorySlot.inventoryItem == null)
            return;

        var item = inventorySlot.inventoryItem;

        if (item.hasInnerPanel)
        {
            innerPanelManager.OpenInnerPanel(item.innerPanelId);
            int innerPanelId = item.innerPanelId;
            HintDetail currentHintDetai = HintManager.instance.GetHintDetail(InnerPanelManager.instance.InnerPanels[innerPanelId].hintId);
            HintManager.instance.selectedHint = currentHintDetai;
            HintManager.instance.currentHintId=HintManager.instance.selectedHint.nextHintId;
        }

        EscapeRoomManager.instance.ShowAlphaText(item.itemName);
    }
}