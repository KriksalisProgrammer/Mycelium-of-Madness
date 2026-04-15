using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class RulesManager : MonoBehaviour
{
    public static RulesManager Instance { get; private set; }

    [Header("All Rules in the game")]
    public List<Rule> allRules = new();

    public UnityEvent<Rule> OnRuleViolated;
    public UnityEvent<Rule> OnRuleActivated;

    private readonly HashSet<string> _violatedRules = new();

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


    public void ViolateRule(string ruleId)
    {
        Rule rule = GetRule(ruleId);
        if (rule == null)
        {
            Debug.LogWarning($"[RulesManager] Rule '{ruleId}' not found.");
            return;
        }

        if (!rule.isActive)
        {
            Debug.Log($"[RulesManager] Rule '{ruleId}' is not active. Violation ignored.");
            return;
        }

        if (_violatedRules.Contains(ruleId))
        {
          
            return;
        }

        _violatedRules.Add(ruleId);
        Debug.Log($"[RulesManager] Rule violated → {rule.ruleText} (Danger: {rule.dangerLevel})");

        OnRuleViolated?.Invoke(rule);
    }

    public void SetRuleActive(string ruleId, bool active)
    {
        Rule rule = GetRule(ruleId);
        if (rule == null) return;

        rule.isActive = active;

        if (active)
        {
            Debug.Log($"[RulesManager] Rule activated: {rule.ruleText}");
            OnRuleActivated?.Invoke(rule);
        }
    }

    public bool WasViolated(string ruleId) => _violatedRules.Contains(ruleId);
    public bool IsRuleActive(string ruleId)
    {
        Rule rule = GetRule(ruleId);
        return rule != null && rule.isActive;
    }

    public int ViolationCount => _violatedRules.Count;

    public Rule GetRule(string ruleId)
    {
        return allRules.Find(r => r.ruleId == ruleId);
    }
    
    public void ResetViolations()
    {
        _violatedRules.Clear();
    }

    public void AddRule(Rule newRule)
    {
        if (newRule == null || allRules.Contains(newRule)) return;
        allRules.Add(newRule);
    }
}