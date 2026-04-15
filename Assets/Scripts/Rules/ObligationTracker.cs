using UnityEngine;
using System.Collections.Generic;

public class ObligationTracker : MonoBehaviour
{
    public static ObligationTracker Instance { get; private set; }

    [Tooltip("Master rule that triggers if ANY obligation is not fulfilled")]
    public string masterRuleId = "unfulfilled_ranger_duty";

    private readonly List<IObligationSource> _sources = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnNightStart.AddListener(CheckAllObligations);
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnNightStart.RemoveListener(CheckAllObligations);
    }

    public void Register(IObligationSource source)
    {
        if (source == null || _sources.Contains(source)) return;
        _sources.Add(source);
    }

    public void Unregister(IObligationSource source)
    {
        _sources.Remove(source);
    }

    private void CheckAllObligations()
    {
        bool anyFailed = false;

        foreach (var src in _sources)
        {
            if (!src.IsFulfilled)
            {
                anyFailed = true;
                Debug.Log($"[ObligationTracker] Violated: {src.ObligationId}");
                
                RulesManager.Instance?.ViolateRule(src.ObligationId);   
            }
        }

        if (anyFailed)
        {
            Debug.Log($"[ObligationTracker] Master rule violated: {masterRuleId}");
            RulesManager.Instance?.ViolateRule(masterRuleId);
        }
    }

    public void ResetDay()
    {
        foreach (var src in _sources)
            src.ResetForNewDay();
    }

    // Полезный метод для отладки
    public IReadOnlyList<IObligationSource> GetAllObligations() => _sources;
}