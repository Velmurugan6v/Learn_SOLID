using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class InventoryItemData
{
    public string itemName;
    public Sprite icon;
    public int id;
    public bool hasInnerPanel;
    public int innerPanelId;
    public Button collectButton;
    public InteractionState interactionState;
    public UnityEvent onCollected;
}