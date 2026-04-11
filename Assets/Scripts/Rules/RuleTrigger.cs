using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class RuleTrigger: MonoBehaviour
    {
        [Header("Rule Trigger")]
        public string ruleid;
        
        public bool checkTimeOfDay = false;
        public float violatedTimeAfterHours = 20f;
        public float violatedTimeBeforeHours = 6f;

        public bool triggerOnce = true;
        public bool triggered =false;

        private void OnTriggerEnter2D(Collider other)
        {
            if(other.CompareTag("Player")) return;
            if(triggerOnce &&triggered) return;
            if(checkTimeOfDay && !IsViolationTime()) return;
            
            triggered = true;
            RulesManager.Instance.ViolentRule(ruleid);
        }

        private bool IsViolationTime()
        {
            if (TimeManager.Instance == null) return true;

            float hour = TimeManager.Instance.CurrentHour;

            if (violatedTimeAfterHours > violatedTimeBeforeHours)
            {
                return hour >= violatedTimeAfterHours || hour < violatedTimeBeforeHours;
            }
            
            return hour >= violatedTimeAfterHours && hour < violatedTimeBeforeHours;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.25f);
            Gizmos.DrawCube(transform.position, transform.localScale);
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
