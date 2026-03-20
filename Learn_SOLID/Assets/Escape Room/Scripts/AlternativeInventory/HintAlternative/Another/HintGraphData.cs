// ═══════════════════════════════════════════════════════════════════
//  HintGraphData.cs
//  ScriptableObject — saved asset that holds all nodes + connections
//  Create via: Assets → Create → HintSystem → Hint Graph
// ═══════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HintEditorOne
{



    [CreateAssetMenu(menuName = "HintSystem/Hint Graph", fileName = "NewHintGraph")]
    public class HintGraphData : ScriptableObject
    {
        public List<HintNodeData> nodes = new List<HintNodeData>();
        public List<HintConnection> connections = new List<HintConnection>();

        // ── Editor helpers ──────────────────────────────────────────
        public HintNodeData GetNodeById(string id) =>
            nodes.Find(n => n.nodeId == id);

        public List<HintConnection> GetConnectionsFrom(string nodeId) =>
            connections.FindAll(c => c.fromNodeId == nodeId && c.fromPortIndex >= 0);

        public HintConnection GetConnectionFromPort(string nodeId, int portIndex) =>
            connections.Find(c => c.fromNodeId == nodeId && c.fromPortIndex == portIndex);
    }

// ─────────────────────────────────────────────────────────────────
//  Node Data — serialized, stored inside HintGraphData
// ─────────────────────────────────────────────────────────────────

    [Serializable]
    public class HintNodeData
    {
        public string nodeId; // GUID
        public HintNodeType nodeType;
        public Rect editorRect; // position/size on canvas
        public string label;
        public Transform hintLocation;

        // ── HintSequence fields ──
        public HintType hintType;
        public int inventoryId;
        public int innerPanelId;
        public float glowDuration = 1.2f;

        // ── Condition fields ──
        [FormerlySerializedAs("typeCondition")] [FormerlySerializedAs("conditionTpye")]
        public ConditionType conditionType;

        public int conditionTargetId; // item id / puzzle id
        public string conditionKey; // for custom flag checks

        // ── Inventory Trigger fields ──
        public int triggerInventoryId;
        public bool triggerOnPickup = true;

        // ── Runtime state (not saved between sessions) ──
        [NonSerialized] public bool isCompleted = false;
        [NonSerialized] public bool isActive = false;
    }

    [Serializable]
    public class HintConnection
    {
        public string fromNodeId;
        public int fromPortIndex; // 0 = default/yes, 1 = no (for conditions)
        public string toNodeId;
    }

// ─────────────────────────────────────────────────────────────────
//  Enums
// ─────────────────────────────────────────────────────────────────

    public enum HintNodeType
    {
        Start,
        HintSequence, // show glow on target
        Condition, // branch: yes / no
        InventoryTrigger, // auto-advance when item picked up
        End
    }

    public enum ConditionType
    {
        ItemCollected,
        PuzzleSolved,
        CustomFlag
    }
}
// Keep existing HintType enum compatible with your HintManager
// public enum HintType { Bg, CloseShot, InnerPanel, Inventory, Custome }