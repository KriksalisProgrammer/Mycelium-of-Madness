using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class RulesManager:MonoBehaviour
{
    public static RulesManager Instance { get; private set; }

    public List<Rule> allRules = new();

    public UnityEvent<Rule> OnRuleViolated;
    public UnityEvent<Rule> OnRuleActivated;
    
    private readonly HashSet<string> violatedRules = new();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {return;}

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ViolentRule(string ruleid)
    {
        Rule rule = GetRule(ruleid);
        
        if (rule == null)
        {
            Debug.LogWarning($"[RulesManager] Rule '{ruleid}' not found.");
            return; 
        }

        if (!rule.isActive)
        {
            Debug.Log($"[RulesManager] Rule '{ruleid}' not actived — violent not score.");
            return;
        }

        if (violatedRules.Contains(rule.ruleId))
        {
            Debug.LogWarning($"[RulesManager] Rule '{ruleid}' is already violated.");
            return;
        }
        violatedRules.Add(ruleid);
        Debug.Log($"[RulesManager] Rule '{rule.ruleText}' is violated: {rule.dangerLevel}");
        OnRuleViolated?.Invoke(rule);
    }

    public void SetActiveRule(string ruleid, bool active)
    {
        Rule rule = GetRule(ruleid);
        
        if(rule == null) return;
        
        rule.isActive = active;
        if (active)
        {
            Debug.Log($"[RulesManager] Rule '{ruleid}' is active : {rule.ruleText}");
            OnRuleActivated?.Invoke(rule);
        }
    }
    
    public bool WasViolated(string ruleid)=> violatedRules.Contains(ruleid);

    public bool IsActive(string ruleid)
    {
        Rule rule = GetRule(ruleid);
        return rule!=null && rule.isActive;
    }

    public int ViolentionCount => violatedRules.Count;
    public Rule GetRule(string ruleid)
    {
        return allRules.Find(r =>r.ruleId == ruleid );
    }
}
