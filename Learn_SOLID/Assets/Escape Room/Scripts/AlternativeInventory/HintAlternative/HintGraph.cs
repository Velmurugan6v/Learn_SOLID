using System.Collections.Generic;
using UnityEngine;

// ─────────────────────────────────────────────────────────────
//  HintGraph.cs
//  The ScriptableObject asset. Create one per puzzle/scene.
//  Right-click in Project → Create → HintGraph → Hint Graph
// ─────────────────────────────────────────────────────────────

[CreateAssetMenu(menuName = "HintGraph/Hint Graph", fileName = "NewHintGraph")]
public class HintGraph : ScriptableObject
{
    [Tooltip("All nodes belonging to this graph (stored as sub-assets)")]
    public List<HintNodeBase> nodes = new();

    [Tooltip("GUID of the StartNode — set automatically by the editor")]
    public string startNodeId;

    // ── Editor-only metadata ──
    [HideInInspector] public Vector2 editorScrollOffset;
    [HideInInspector] public float   editorZoom = 1f;

    // ────────────────────────────────────────
    //  Accessors
    // ────────────────────────────────────────

    public HintNodeBase GetNode(string nodeId) =>
        nodes.Find(n => n.nodeId == nodeId);

    public HintNodeBase GetStartNode()
    {
        if (!string.IsNullOrEmpty(startNodeId))
            return GetNode(startNodeId);

        // Fallback: find first StartNode
        return nodes.Find(n => n is StartNode);
    }

    public T AddNode<T>(Vector2 position) where T : HintNodeBase
    {
        var node            = CreateInstance<T>();
        node.nodeId         = System.Guid.NewGuid().ToString();
        node.editorPosition = position;
        node.name           = typeof(T).Name;
        nodes.Add(node);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.AddObjectToAsset(node, this);
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        return node;
    }

    public void RemoveNode(HintNodeBase node)
    {
        // Remove all connections pointing to this node
        foreach (var n in nodes)
            n.outputs.RemoveAll(c => c.targetNodeId == node.nodeId);

        nodes.Remove(node);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.RemoveObjectFromAsset(node);
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    public void Connect(HintNodeBase from, string portLabel, HintNodeBase to)
    {
        // Remove existing connection on same port first
        from.outputs.RemoveAll(c => c.portLabel == portLabel);
        from.outputs.Add(new HintNodeConnection
        {
            portLabel    = portLabel,
            targetNodeId = to.nodeId
        });

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public void Disconnect(HintNodeBase from, string portLabel)
    {
        from.outputs.RemoveAll(c => c.portLabel == portLabel);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    /// <summary>Reset all node runtime states (call on scene load)</summary>
    public void ResetAllNodes()
    {
        foreach (var node in nodes)
            node.ResetState();
    }
}
