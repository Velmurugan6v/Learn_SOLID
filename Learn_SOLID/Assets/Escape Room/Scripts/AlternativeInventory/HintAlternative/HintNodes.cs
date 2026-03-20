using UnityEngine;

// ═════════════════════════════════════════════════════════════
//  HintNodes.cs  —  UPDATED
//
//  KEY CHANGE: All nodes now use a plain string hintTargetId
//  instead of a Button/Transform reference.
//
//  WHY: ScriptableObjects cannot hold scene object references.
//  The string ID is resolved at runtime via HintTargetRegistry.
//
//  HOW TO SET UP IN INSPECTOR:
//  - On your node asset: set hintTargetId = "door_key_button"
//  - In your scene: add HintTarget component to the Button,
//    set its hintId = "door_key_button"
//  That's it. No more dragging scene objects into assets.
// ═════════════════════════════════════════════════════════════

// ─────────────────────────────────────────────────────────────
//  StartNode
// ─────────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "HintGraph/Nodes/Start")]
public class StartNode : HintNodeBase
{
    public override void   Execute(HintGraphRunner r) { }
    public override bool   IsComplete()               => true;
    public override string GetNodeTitle()             => "START";
}

// ─────────────────────────────────────────────────────────────
//  BgHintNode  — highlights a world-space object on the BG
// ─────────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "HintGraph/Nodes/Bg Hint")]
public class BgHintNode : HintNodeBase
{
    [Tooltip("Match this to a HintTarget.hintId in your scene")]
    public string hintTargetId;

    public float glowDuration = 1.2f;

    public override string GetNodeTitle() => "BG Hint";

    public override void Execute(HintGraphRunner runner)
    {
        if (EscapeRoomManager.instance.isCloseShotOpened)
            EscapeRoomManager.instance.CloseShotNoAnimation();

        Transform target = HintTargetRegistry.Instance.Resolve(hintTargetId);
        if (target != null)
            runner.ShowGlow(target, glowDuration, this);
    }

    public override bool IsComplete() => isCompleted;
}

// ─────────────────────────────────────────────────────────────
//  CloseShotHintNode
// ─────────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "HintGraph/Nodes/CloseShot Hint")]
public class CloseShotHintNode : HintNodeBase
{
    [Tooltip("Match this to a HintTarget.hintId in your scene")]
    public string hintTargetId;

    public float glowDuration = 1.2f;

    public override string GetNodeTitle() => "CloseShot Hint";

    public override void Execute(HintGraphRunner runner)
    {
        Transform target = HintTargetRegistry.Instance.Resolve(hintTargetId);
        if (target != null)
            runner.ShowGlow(target, glowDuration, this);
    }

    public override bool IsComplete() => isCompleted;
}

// ─────────────────────────────────────────────────────────────
//  InventoryHintNode
// ─────────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "HintGraph/Nodes/Inventory Hint")]
public class InventoryHintNode : HintNodeBase
{
    [Tooltip("Match this to a HintTarget.hintId in your scene")]
    public string hintTargetId;

    [Tooltip("Also used to search InventoryObjectManager as fallback")]
    public int inventoryItemId;

    public float glowDuration = 1.2f;

    public override string GetNodeTitle() => "Inventory Hint";

    public override void Execute(HintGraphRunner runner)
    {
        // Primary: use registry ID
        Transform target = HintTargetRegistry.Instance.Resolve(hintTargetId);

        // Fallback: search inventory slots by item id
        if (target == null)
            target = FindInventorySlot(inventoryItemId);

        if (target != null)
            runner.ShowGlow(target, glowDuration, this);
    }

    private Transform FindInventorySlot(int itemId)
    {
        foreach (var slot in InventoryObjectManager.instance.inventorySlots)
            if (slot.inventoryItem != null && slot.inventoryItem.id == itemId)
                return slot.transform;
        return null;
    }

    public override bool IsComplete() => isCompleted;
}

// ─────────────────────────────────────────────────────────────
//  InnerPanelHintNode
// ─────────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "HintGraph/Nodes/InnerPanel Hint")]
public class InnerPanelHintNode : HintNodeBase
{
    [Tooltip("Match this to a HintTarget.hintId in your scene")]
    public string hintTargetId;

    public int   innerPanelId;
    public float glowDuration = 1.2f;

    public override string GetNodeTitle() => "InnerPanel Hint";

    public override void Execute(HintGraphRunner runner)
    {
        Transform target = HintTargetRegistry.Instance.Resolve(hintTargetId);
        if (target != null)
            runner.ShowGlow(target, glowDuration, this);
    }

    public override bool IsComplete() => isCompleted;
}

// ─────────────────────────────────────────────────────────────
//  ConditionNode  — branches: True port or False port
// ─────────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "HintGraph/Nodes/Condition")]
public class ConditionNode : HintNodeBase
{
    public enum ConditionType { InventoryHasItem, CustomFlag }

    public ConditionType conditionType;
    public int           inventoryItemId;
    public string        customFlagKey;

    public override string GetNodeTitle() => "Condition";

    public override void Execute(HintGraphRunner runner)
    {
        bool result    = Evaluate();
        string port    = result ? "True" : "False";
        runner.AdvanceFromPort(this, port);
    }

    private bool Evaluate()
    {
        switch (conditionType)
        {
            case ConditionType.InventoryHasItem:
                foreach (var slot in InventoryObjectManager.instance.inventorySlots)
                    if (slot.inventoryItem != null && slot.inventoryItem.id == inventoryItemId)
                        return true;
                return false;

            case ConditionType.CustomFlag:
                return HintGraphRunner.GetFlag(customFlagKey);

            default: return false;
        }
    }

    public override bool IsComplete() => true;
}

// ─────────────────────────────────────────────────────────────
//  EndNode
// ─────────────────────────────────────────────────────────────
[CreateAssetMenu(menuName = "HintGraph/Nodes/End")]
public class EndNode : HintNodeBase
{
    public override string GetNodeTitle() => "END";

    public override void Execute(HintGraphRunner runner)
    {
        Debug.Log($"[HintGraph] Graph '{runner.graph.name}' complete.");
        runner.OnGraphComplete?.Invoke();
    }

    public override bool IsComplete() => true;
}