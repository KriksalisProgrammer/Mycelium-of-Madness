using UnityEngine;
using System.Collections.Generic;

public class AltarWatererObligation : MonoBehaviour, IObligationSource
{
    [Header("Responsibility: altars")]
    public int requiredAltars = 1;

    private readonly HashSet<string> _wateredAltars = new();

    public string ObligationId => "dry_altars";
    public bool IsFulfilled => _wateredAltars.Count >= requiredAltars;

    void OnEnable() => ObligationTracker.Instance?.Register(this);
    void OnDisable() => ObligationTracker.Instance?.Unregister(this);

    public void WaterAltar(string altarId)
    {
        _wateredAltars.Add(altarId);
        Debug.Log($"[Altar] Altar cloths: {_wateredAltars.Count}/{requiredAltars}");
    }

    public void ResetForNewDay() => _wateredAltars.Clear();
}