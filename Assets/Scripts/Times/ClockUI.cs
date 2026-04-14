using UnityEngine;
using UnityEngine.UI;
using TMPro;                    
using UnityEngine.Events;
    public class ClockUI:MonoBehaviour
    {
       
        [SerializeField] private TextMeshProUGUI timeText;          
        [SerializeField] private TextMeshProUGUI dayText;            
        [SerializeField] private float madnessShakeAmount = 1.5f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color madnessColor = new Color(1f, 0.3f, 0.3f);
        
        private RectTransform rectTransform;
        private float originalFontSize;
        private bool isMadnessMode = false;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        
            if (timeText != null)
                originalFontSize = timeText.fontSize;
        }
        private void OnEnable()
        {
            if (TimeManager.Instance == null) 
            {
                Debug.LogWarning("TimeManager не найден!");
                return;
            }

            
            TimeManager.Instance.OnMinutePassed.AddListener(UpdateTimeDisplay);
            TimeManager.Instance.OnDayPassed.AddListener(UpdateDayDisplay);
            TimeManager.Instance.OnPhaseChanged.AddListener(OnInfectionPhaseChanged);

            // Первичное обновление
            UpdateTimeDisplay();
            UpdateDayDisplay();
        }
        private void OnDisable()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnMinutePassed.RemoveListener(UpdateTimeDisplay);
                TimeManager.Instance.OnDayPassed.RemoveListener(UpdateDayDisplay);
                TimeManager.Instance.OnPhaseChanged.RemoveListener(OnInfectionPhaseChanged);
            }
        }
        private void UpdateTimeDisplay()
        {
            if (timeText == null) return;

            int hours = TimeManager.Instance.CurrentHour;
            int minutes = TimeManager.Instance.CurrentMinute;
            timeText.text = $"{hours:00}:{minutes:00}";
            
            ApplyMadnessEffects();
        }
        private void UpdateDayDisplay()
        {
            if (dayText == null) return;
            dayText.text = $"День {TimeManager.Instance.CurrentDay}";
        }
        private void OnInfectionPhaseChanged(TimeManager.InfectionPhase phase)
        {
            
            switch (phase)
            {
                case TimeManager.InfectionPhase.Early:
                    timeText.color = normalColor;
                    break;
                case TimeManager.InfectionPhase.Advanced:
                    timeText.color = Color.Lerp(normalColor, madnessColor, 0.5f);
                    break;
                case TimeManager.InfectionPhase.Critical:
                    timeText.color = madnessColor;
                    isMadnessMode = true;
                    break;
            }
        }
        private void ApplyMadnessEffects()
        {
            if (!isMadnessMode || timeText == null) return;
            
            float shake = Mathf.Sin(Time.time * 15f) * madnessShakeAmount * 0.1f;
            rectTransform.anchoredPosition = new Vector2(shake, shake);
            
            float pulse = 1f + Mathf.Sin(Time.time * 8f) * 0.08f;
            timeText.fontSize = originalFontSize * pulse;
        }
        public void SetMadnessIntensity(float intensity)
        {
            isMadnessMode = intensity > 0.6f;
            madnessShakeAmount = intensity * 3f;
        }
    }
