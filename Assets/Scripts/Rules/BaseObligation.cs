using UnityEngine;

public abstract class BaseObligation : MonoBehaviour, IObligationSource
{
    [Header("Obligation Settings")]
    [SerializeField] protected string obligationId = "default_obligation";
    [SerializeField] protected int requiredCount = 1;

    protected int currentProgress = 0;

    public string ObligationId => obligationId;
    
    public virtual bool IsFulfilled => currentProgress >= requiredCount;

    public virtual void ResetForNewDay()
    {
        currentProgress = 0;
        OnProgressChanged();
    }
    
    public virtual void AddProgress(int amount = 1)
    {
        if (IsFulfilled) return;
        
        currentProgress += amount;
        OnProgressChanged();

        if (IsFulfilled)
            OnFulfilled();
    }
    
    protected virtual void OnProgressChanged() { }
    protected virtual void OnFulfilled() 
    {
        Debug.Log($"[Obligation] Fulfilled: {obligationId}");
    }
    
    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(obligationId))
            obligationId = GetType().Name;
    }
}