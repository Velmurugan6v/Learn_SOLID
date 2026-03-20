using System.Collections.Generic;
using UnityEngine;

// ─────────────────────────────────────────────────────────────
//  HintTargetRegistry.cs
//
//  WHY THIS EXISTS:
//  ScriptableObjects live on disk (Project) — they cannot hold
//  references to scene objects like Button or Transform.
//  Solution: store a plain string ID in the node asset.
//  Each scene object registers its Transform here at runtime.
//  The HintGraphRunner resolves the ID → Transform when hinting.
//
//  SETUP:
//  1. Add HintTargetRegistry to a persistent GameObject in your scene.
//  2. Add HintTarget component to every object that can be hinted.
//  3. Set the hintId string on each HintTarget to match your node's hintTargetId.
// ─────────────────────────────────────────────────────────────

public class HintTargetRegistry : MonoBehaviour
{
    public static HintTargetRegistry Instance { get; private set; }

    private readonly Dictionary<string, Transform> _registry = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>Called by HintTarget.OnEnable — registers a scene transform under an ID.</summary>
    public void Register(string id, Transform target)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("[HintTargetRegistry] Tried to register with empty id.");
            return;
        }
        _registry[id] = target;
    }

    /// <summary>Called by HintTarget.OnDisable.</summary>
    public void Unregister(string id) => _registry.Remove(id);

    /// <summary>Returns the Transform for the given ID, or null if not found.</summary>
    public Transform Resolve(string id)
    {
        if (_registry.TryGetValue(id, out Transform t)) return t;
        Debug.LogWarning($"[HintTargetRegistry] No target registered with id='{id}'. " +
                         $"Make sure a HintTarget with this id is active in the scene.");
        return null;
    }

    public bool Has(string id) => _registry.ContainsKey(id);
}
