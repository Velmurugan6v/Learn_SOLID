using UnityEngine;

// ─────────────────────────────────────────────────────────────
//  HintTarget.cs
//
//  Attach this to any scene object that a hint node should
//  point at — a Button, a prop, a UI panel, anything.
//
//  Set hintId to match the hintTargetId field on your node asset.
//  That's the only wiring you need to do in the scene.
// ─────────────────────────────────────────────────────────────

public class HintTarget : MonoBehaviour
{
    [Tooltip("Must match the hintTargetId field in your hint node asset")]
    public string hintId;

    private void OnEnable()
    {
        if (HintTargetRegistry.Instance != null)
            HintTargetRegistry.Instance.Register(hintId, transform);
        else
            Debug.LogWarning($"[HintTarget] '{hintId}' could not register — HintTargetRegistry not found.");
    }

    private void OnDisable()
    {
        if (HintTargetRegistry.Instance != null)
            HintTargetRegistry.Instance.Unregister(hintId);
    }
}
