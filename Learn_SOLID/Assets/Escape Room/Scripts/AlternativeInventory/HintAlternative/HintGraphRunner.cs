using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ─────────────────────────────────────────────────────────────
//  HintGraphRunner.cs  — updated with editor notification
//
//  Calls NotifyEditor() whenever the active node changes so the
//  HintGraphEditorWindow can update its visual state in real time.
// ─────────────────────────────────────────────────────────────

public class HintGraphRunner : MonoBehaviour
{
    [Header("Graph")] public HintGraph graph;

    [Header("UI")] public Button hintButton;
    public GameObject hintGlowPrefab;

    [Header("Timing")] public float glowDuration = 1.2f;
    public float cooldownSeconds = 2.0f;

    public Action OnGraphComplete;
    public Action OnHintUsed;

    public HintNodeBase _currentNode;
    private bool _isShowingGlow;
    private bool _onCooldown;

    private static Dictionary<string, bool> _flags = new();

    private void Awake()
    {
        if (graph == null)
        {
            Debug.LogError("[HintGraphRunner] No HintGraph assigned!");
            return;
        }

        graph.ResetAllNodes();
        _currentNode = graph.GetStartNode();
        AdvanceToNextExecutable();
        NotifyEditor(_currentNode);
    }

    private void OnEnable() => hintButton?.onClick.AddListener(UseHint);
    private void OnDisable() => hintButton?.onClick.RemoveListener(UseHint);

    // ── Public API ───────────────────────────

    public void UseHint()
    {
        if (_isShowingGlow || _onCooldown) return;
        if (_currentNode == null)
        {
            Debug.Log("[HintGraphRunner] Graph complete.");
            return;
        }

        _currentNode.Execute(this);
        OnHintUsed?.Invoke();
    }

    public void CompleteCurrentNode()
    {
        if (_currentNode == null) return;
        _currentNode.isCompleted = true;
        NotifyEditor(_currentNode); // show green on completed node
        AdvanceFromPort(_currentNode, "Next");
    }

    public void CompleteNodeById(string nodeId)
    {
        var node = graph.GetNode(nodeId);
        if (node == null) return;
        node.isCompleted = true;
        NotifyEditor(node);
        if (_currentNode?.nodeId == nodeId)
            AdvanceFromPort(node, "Next");
    }

    public static void SetFlag(string key, bool value) => _flags[key] = value;
    public static bool GetFlag(string key) => _flags.TryGetValue(key, out bool v) && v;

    // ── Graph Traversal ──────────────────────

    public void AdvanceFromPort(HintNodeBase node, string portLabel = "Next")
    {
        var connection = node.outputs.Find(c => c.portLabel == portLabel);
        if (connection == null)
        {
            _currentNode = null;
            NotifyEditor(null);
            return;
        }

        _currentNode = graph.GetNode(connection.targetNodeId);
        if (_currentNode == null)
        {
            NotifyEditor(null);
            return;
        }

        NotifyEditor(_currentNode); // highlight the new active node

        if (_currentNode is ConditionNode || _currentNode is EndNode)
            _currentNode.Execute(this);
        else if (_currentNode.isCompleted)
            AdvanceFromPort(_currentNode, "Next");
    }

    private void AdvanceToNextExecutable()
    {
        int limit = graph.nodes.Count + 1;
        while (_currentNode != null && _currentNode.isCompleted && limit-- > 0)
            AdvanceFromPort(_currentNode, "Next");
    }

    // ── Glow VFX ────────────────────────────

    public void ShowGlow(Transform target, float duration, HintNodeBase sourceNode)
    {
        if (_isShowingGlow) return;
        StartCoroutine(GlowRoutine(target, duration, sourceNode));
    }

    private IEnumerator GlowRoutine(Transform target, float duration, HintNodeBase node)
    {
        _isShowingGlow = true;
        hintGlowPrefab.transform.position = target.position;
        hintGlowPrefab.SetActive(true);

        yield return new WaitForSeconds(duration);

        hintGlowPrefab.SetActive(false);
        _onCooldown = true;
        yield return new WaitForSeconds(cooldownSeconds);
        _onCooldown = false;
        _isShowingGlow = false;
    }

    // ── Editor Bridge ────────────────────────

    /// <summary>
    /// Writes the active node ID into HintGraphBridge so the editor window
    /// can read it. HintGraphBridge is a plain Runtime class — no Editor
    /// dependency, compiles fine in builds (the data is just never read).
    /// </summary>
    private void NotifyEditor(HintNodeBase activeNode)
    {
        HintGraphBridge.SetActiveNode(activeNode?.nodeId);
    }
}