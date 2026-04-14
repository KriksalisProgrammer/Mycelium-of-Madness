using UnityEngine;
using System.Collections.Generic;

public class TrailCheckerObligation : MonoBehaviour, IObligationSource
{
    [Header("Responsibility: Trails")]
    public int trailCount = 3;

    private readonly HashSet<string> _checkedTrails = new();

    public string ObligationId => "open_trails";
    public bool IsFulfilled => _checkedTrails.Count >= trailCount;

    void OnEnable() => ObligationTracker.Instance?.Register(this);
    void OnDisable() => ObligationTracker.Instance?.Unregister(this);

    public void CheckTrail(string trailId)
    {
        _checkedTrails.Add(trailId);
        Debug.Log($"[Trail] Trails checked: {_checkedTrails.Count}/{trailCount}");
    }

    public void ResetForNewDay() => _checkedTrails.Clear();
}