
using UnityEngine; 


    [CreateAssetMenu(fileName = "New Rule", menuName = "Rules/Rule")]
    public class Rule: ScriptableObject
    {
        [Header("Main")]
        public string ruleId;
        public string ruleText;
        public bool isActive;

        [TextArea(2,4)]
        public string violentionNote;
        public float dangerLevel = 1f;
    }
