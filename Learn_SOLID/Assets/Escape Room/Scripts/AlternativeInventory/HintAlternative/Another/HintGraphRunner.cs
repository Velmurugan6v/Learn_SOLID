// ═══════════════════════════════════════════════════════════════════
//  HintGraphRunner.cs
//  Attach to the same GameObject as your HintManager.
//  Walks the HintGraphData at runtime when player taps Hint button.
// ═══════════════════════════════════════════════════════════════════

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HintEditorOne
{



    public class HintGraphRunner : MonoBehaviour
    {
        public static HintGraphRunner instance;

        [Header("Graph Asset")] public HintGraphData graph;

        [Header("Glow VFX")] public GameObject hintGlowPrefab; // instantiated at target position

        [Header("Timing")] public float glowDuration = 1.2f;
        public float postHintDelay = 0.5f;

        // ── State ────────────────────────────────────────────────────
        public HintNodeData _currentNode;
        private bool _isRunning = false;
        private bool _isBlocked = false; // cooldown / animation lock

        // External condition registry — other systems register here
        private HashSet<string> _completedFlags = new HashSet<string>();
        private HashSet<int> _collectedItems = new HashSet<int>();
        private HashSet<int> _solvedPuzzles = new HashSet<int>();

        // ─────────────────────────────────────────────────────────────

        #region Unity

        // ─────────────────────────────────────────────────────────────

        private void Awake() => instance = this;

        private void Start() => ResetGraph();

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Public API — called by HintManager or UI button

        // ─────────────────────────────────────────────────────────────

        /// <summary>Called when player presses the Hint button.</summary>
        public void RequestHint()
        {
            if (_isBlocked)
            {
                Debug.Log("[HintRunner] Hint blocked — animation running or on cooldown.");
                return;
            }

            if (_currentNode == null)
            {
                Debug.LogWarning("[HintRunner] No current node. Did you call ResetGraph()?");
                return;
            }

            StartCoroutine(ExecuteNode(_currentNode));
        }

        /// <summary>Reset the graph to the Start node (call on scene load).</summary>
        public void ResetGraph()
        {
            if (graph == null)
            {
                Debug.LogError("[HintRunner] No HintGraphData assigned!");
                return;
            }

            // Reset all node runtime state
            foreach (var n in graph.nodes)
            {
                n.isCompleted = false;
                n.isActive = false;
            }

            _currentNode = FindStartNode();
            if (_currentNode == null) Debug.LogError("[HintRunner] No Start node found in graph.");
        }

        // ── Condition setters (called by your game systems) ──────────

        public void SetItemCollected(int itemId) => _collectedItems.Add(itemId);
        public void SetPuzzleSolved(int puzzleId) => _solvedPuzzles.Add(puzzleId);
        public void SetFlag(string flag) => _completedFlags.Add(flag);
        public void ClearFlag(string flag) => _completedFlags.Remove(flag);

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Node Execution

        // ─────────────────────────────────────────────────────────────

        private IEnumerator ExecuteNode(HintNodeData node)
        {
            _isBlocked = true;
            node.isActive = true;

            switch (node.nodeType)
            {
                case HintNodeType.Start:
                    yield return ExecuteStart(node);
                    break;

                case HintNodeType.HintSequence:
                    yield return ExecuteHintSequence(node);
                    break;

                case HintNodeType.Condition:
                    yield return ExecuteCondition(node);
                    break;

                case HintNodeType.InventoryTrigger:
                    yield return ExecuteInventoryTrigger(node);
                    break;

                case HintNodeType.End:
                    ExecuteEnd(node);
                    break;
            }

            node.isActive = false;
            node.isCompleted = true;
            _isBlocked = false;
        }

        // ── Start ─────────────────────────────────────────────────────
        private IEnumerator ExecuteStart(HintNodeData node)
        {
            // Immediately pass through to the next node
            HintNodeData next = GetOutputNode(node, 0);
            if (next != null)
            {
                _currentNode = next;
                yield return ExecuteNode(next);
            }

            yield return null;
        }

        // ── HintSequence ──────────────────────────────────────────────
        private IEnumerator ExecuteHintSequence(HintNodeData node)
        {
            Transform target = ResolveTarget(node);

            if (target == null)
            {
                Debug.LogWarning($"[HintRunner] HintSequence '{node.label}': could not resolve target.");
                AdvanceToNext(node, 0);
                yield break;
            }

            // Show glow at target
            yield return ShowGlow(target, node.glowDuration);

            // Auto-advance in sequence
            AdvanceToNext(node, 0);
            yield return new WaitForSeconds(postHintDelay);
        }

        // ── Condition ─────────────────────────────────────────────────
        private IEnumerator ExecuteCondition(HintNodeData node)
        {
            bool result = EvaluateCondition(node);

            // Port 0 = YES/TRUE output, Port 1 = NO/FALSE output
            int port = result ? 0 : 1;
            AdvanceToNext(node, port);

            yield return null;
        }

        // ── InventoryTrigger ──────────────────────────────────────────
        private IEnumerator ExecuteInventoryTrigger(HintNodeData node)
        {
            // Highlight the inventory slot so player knows what to pick up
            Transform slot = FindInventorySlot(node.triggerInventoryId);

            if (slot != null)
                yield return ShowGlow(slot, 1.5f);

            // Wait until item is actually collected before advancing
            float timeout = 60f;
            float elapsed = 0f;

            while (!_collectedItems.Contains(node.triggerInventoryId) && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            AdvanceToNext(node, 0);
        }

        // ── End ───────────────────────────────────────────────────────
        private void ExecuteEnd(HintNodeData node)
        {
            Debug.Log("[HintRunner] Hint graph reached End node — all hints complete.");
            _currentNode = null;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────

        #region Helpers

        // ─────────────────────────────────────────────────────────────

        private void AdvanceToNext(HintNodeData from, int portIndex)
        {
            HintNodeData next = GetOutputNode(from, portIndex);
            _currentNode = next; // null = end of graph, that's fine
        }

        private HintNodeData GetOutputNode(HintNodeData from, int portIndex)
        {
            HintConnection conn = graph.GetConnectionFromPort(from.nodeId, portIndex);
            if (conn == null) return null;
            return graph.GetNodeById(conn.toNodeId);
        }

        private HintNodeData FindStartNode() =>
            graph.nodes.Find(n => n.nodeType == HintNodeType.Start);

        // ── Condition evaluation ──────────────────────────────────────
        private bool EvaluateCondition(HintNodeData node)
        {
            switch (node.conditionType)
            {
                case ConditionType.ItemCollected:
                    return _collectedItems.Contains(node.conditionTargetId);

                case ConditionType.PuzzleSolved:
                    return _solvedPuzzles.Contains(node.conditionTargetId);

                case ConditionType.CustomFlag:
                    return _completedFlags.Contains(node.conditionKey);

                default:
                    return false;
            }
        }

        // ── Target resolution ─────────────────────────────────────────
        private Transform ResolveTarget(HintNodeData node)
        {
            switch (node.hintType)
            {
                case HintType.Bg:
                    return node.hintLocation;
                case HintType.CloseShot:
                    // Find a GameObject tagged or named with the hint label
                    GameObject go = GameObject.Find(node.label);
                    return go != null ? go.transform : null;

                case HintType.Inventory:
                    return FindInventorySlot(node.inventoryId);

                case HintType.InnerPanel:
                    return FindInnerPanel(node.innerPanelId);

                default:
                    return null;
            }
        }

        private Transform FindInventorySlot(int itemId)
        {
            if (InventoryObjectManager.instance == null) return null;
            foreach (var slot in InventoryObjectManager.instance.inventorySlots)
                if (slot.inventoryItem != null && slot.inventoryItem.id == itemId)
                    return slot.transform;
            return null;
        }

        private Transform FindInnerPanel(int panelId)
        {
            // Wire to your InnerPanelManager
            // return InnerPanelManager.instance.GetById(panelId)?.transform;
            Debug.LogWarning($"[HintRunner] FindInnerPanel id={panelId} — wire your panel manager.");
            return null;
        }

        // ── Glow VFX ─────────────────────────────────────────────────
        private IEnumerator ShowGlow(Transform target, float duration)
        {
            GameObject glow = null;

            if (hintGlowPrefab != null)
            {
                glow = Instantiate(hintGlowPrefab, target.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(duration);

            if (glow != null) Destroy(glow);
        }

        #endregion
    }

}