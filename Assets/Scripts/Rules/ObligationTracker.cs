using UnityEngine;
using System.Collections.Generic;

    public class ObligationTracker: MonoBehaviour
    {
        public static ObligationTracker Instance { get; private set; }
        
        [Tooltip("ruleId is violated if even one obligation is not fulfilled")]
        public string masterRuleId = "unfulfilled_ranger_duty";
        
        private readonly List<IObligationSource> _sources = new();
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); return;
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
            if (!_sources.Contains(source))
                _sources.Add(source);
        }

        public void Unregister(IObligationSource source) => _sources.Remove(source);

        private void CheckAllObligations()
        {
            foreach (var src in _sources)
            {
                if (!src.IsFulfilled)
                {
                    Debug.Log($"[ObligationTracker] Not completed: {src.ObligationId}");
                    RulesManager.Instance?.ViolentRule(src.ObligationId);
                }
            }
          
            bool anyFailed = _sources.Exists(s => !s.IsFulfilled);
            if (anyFailed)
                RulesManager.Instance?.ViolentRule(masterRuleId);
        }
        public void ResetDay()
        {
            foreach (var src in _sources)
                src.ResetForNewDay();
        }
    }
