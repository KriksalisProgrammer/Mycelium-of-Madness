using UnityEngine;

public class MushroomCollectorObligation : MonoBehaviour, IObligationSource
{
    [Header("Responsibility: mushrooms")]
    public int requiredCount = 10;

    private int _collected;

    public string ObligationId => "underfed_mycelium";
    public bool IsFulfilled => _collected >= requiredCount;

    void OnEnable() => ObligationTracker.Instance?.Register(this);
    void OnDisable() => ObligationTracker.Instance?.Unregister(this);

    public void CollectMushroom(int amount = 1)
    {
        _collected += amount;
        Debug.Log($"[Mushroom] Compile : {_collected}/{requiredCount}");
    }

    public void ResetForNewDay() => _collected = 0;
}