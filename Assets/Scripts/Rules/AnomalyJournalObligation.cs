using UnityEngine;

public class AnomalyJournalObligation : MonoBehaviour, IObligationSource
{
    [Header("Responsibility: anomalies")]
    public int requiredEntries = 1;

    private int _entries;

    public string ObligationId => "unrecorded_anomalies";
    public bool IsFulfilled => _entries >= requiredEntries;

    void OnEnable() => ObligationTracker.Instance?.Register(this);
    void OnDisable() => ObligationTracker.Instance?.Unregister(this);

    public void RecordAnomaly()
    {
        _entries++;
        Debug.Log($"[Journal] Anomalies recorded: {_entries}");
    }

    public void ResetForNewDay() => _entries = 0;
}