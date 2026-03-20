using System;
using System.Collections.Generic;
using UnityEngine;

// ─────────────────────────────────────────────────────────────
//  HintNodeBase.cs
//  Base class for every node in the hint graph.
//  Stored as a sub-asset inside HintGraph ScriptableObject.
// ─────────────────────────────────────────────────────────────

[Serializable]
public abstract class HintNodeBase : ScriptableObject
{
    [HideInInspector] public string   nodeId;          // GUID
    [HideInInspector] public Vector2  editorPosition;  // canvas position
    [HideInInspector] public List<HintNodeConnection> outputs = new();

    // ── Runtime state (not serialized — reset each play) ──
    [NonSerialized] public bool isCompleted = false;

    /// <summary>Called by HintManager when this node is the active hint.</summary>
    public abstract void Execute(HintGraphRunner runner);

    /// <summary>Return true when this node's completion condition is met.</summary>
    public abstract bool IsComplete();

    /// <summary>Human-readable label shown in the editor node header.</summary>
    public abstract string GetNodeTitle();

    public virtual void ResetState() => isCompleted = false;
}

// ─────────────────────────────────────────────────────────────
//  Connection between two nodes (output port → input of next)
// ─────────────────────────────────────────────────────────────
[Serializable]
public class HintNodeConnection
{
    public string portLabel;      // e.g. "Next", "On Complete", "On Skip"
    public string targetNodeId;   // GUID of target node
}
