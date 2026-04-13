using UnityEngine;
using UnityEngine.UI;
    public class ClockUI:MonoBehaviour
    {
       
        public Text timeText;
        public Text periodText;
 
        [Header("Color")]
        public Color dayColor   = new Color(1f,   0.9f, 0.6f);
        public Color nightColor = new Color(1f, 0f, 0f);
            
            
        void Update()
        {
            if (TimeManager.Instance == null) return;
            
            timeText.text   = TimeManager.Instance.TimeString;
            periodText.text = TimeManager.Instance.isNight ? "Night" : "Day";
 
            Color c = TimeManager.Instance.isNight ? nightColor : dayColor;
            timeText.color   = c;
            periodText.color = c;
 
 
        }
    }
