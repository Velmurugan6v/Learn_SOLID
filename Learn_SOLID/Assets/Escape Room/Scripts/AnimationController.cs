using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public AnimationParentInfo animationParentInfo;

    private void OnEnable()
    {
        AnimationManager.instance.PlayAutoAnimation(animationParentInfo);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Show ? Interaction Text
        if (animationParentInfo.isDone)
            return;

        if (animationParentInfo.animationChildInfos[animationParentInfo.currentAnimationIndex].canShowText)
            EscapeRoomManager.instance.ShowAlphaText(animationParentInfo
                .animationChildInfos[animationParentInfo.currentAnimationIndex].interactionText);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag.TryGetComponent<InventoryItem>(out var inventoryItem))
        {
            Debug.Log("There is no inventory slot to drag inventory object");
            return;
        }

        if (animationParentInfo.isDone)
            return;

        if (!CanDrag(animationParentInfo, inventoryItem))
            return;

        AnimationManager.instance.PlayAnimation(animationParentInfo, inventoryItem);
        inventoryItem.ResetSlot();
        InventoryObjectManager.instance.RemoveItem(inventoryItem.inventorySlot.inventoryItem);
    }

    public bool CanDrag(AnimationParentInfo animationParentInfo, InventoryItem inventoryItem)
    {
        if (animationParentInfo.animationChildInfos[animationParentInfo.currentAnimationIndex].inventroyId ==
            inventoryItem.inventorySlot.inventoryItem.id)
            return true;

        return false;
    }
}