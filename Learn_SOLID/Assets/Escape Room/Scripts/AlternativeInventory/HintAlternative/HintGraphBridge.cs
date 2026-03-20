// ─────────────────────────────────────────────────────────────
//  HintGraphBridge.cs  —  place in Runtime/ folder
//
//  A pure static class with no UnityEditor dependency.
//  Runtime (HintGraphRunner) writes to it.
//  Editor (HintGraphEditorWindow) reads from it.
//
//  This is the standard Unity pattern for runtime → editor
//  communication without creating a circular assembly reference.
// ─────────────────────────────────────────────────────────────

public static class HintGraphBridge
{
    /// <summary>NodeId of the currently active hint node. Set by HintGraphRunner.</summary>
    public static string ActiveNodeId = null;

    /// <summary>Optional: editor can subscribe to get immediate notification.</summary>
    public static System.Action<string> OnActiveNodeChanged;

    public static void SetActiveNode(string nodeId)
    {
        ActiveNodeId = nodeId;
        OnActiveNodeChanged?.Invoke(nodeId);
    }
}