using System.Collections;
using UnityEngine;
using UnityEngine.UI;

    public class ViolationReactor:MonoBehaviour
    {
        public Image screenOverlay;
        public Text journalText;
        
        public AudioSource audioSource;
        public AudioClip violationSound;
        
        public Color flashColor= new Color(0.4f, 0f, 0f,0.6f);
        public float flashDuration = 1.5f;

        public void OnRuleViolated(Rule rule)
        {
            StartCoroutine(ViolationEffect(rule));
        }

        public IEnumerator ViolationEffect(Rule rule)
        {
            if (audioSource && violationSound)
            {
                audioSource.PlayOneShot(violationSound);
            }

            if (screenOverlay)
            {
                screenOverlay.color = flashColor;
                float t = 0f;
                while (t < flashDuration)
                {
                    t += UnityEngine.Time.deltaTime;
                    float alpha = Mathf.Lerp(flashColor.a,0f , t/flashDuration);
                    screenOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
                    yield return null;
                }
                screenOverlay.color = Color.clear;
            }

            if (journalText && !string.IsNullOrEmpty(rule.violentionNote))
            {
                journalText.text = rule.violentionNote;
            }

            if (rule.dangerLevel >= 2f)
            {
                TriggerWorldReaction(rule);
            }
        }

        private void TriggerWorldReaction(Rule rule)
        {
            Debug.Log($"[ViolationReactor] World Reaction: {rule.ruleId}");
            
        }
    }
