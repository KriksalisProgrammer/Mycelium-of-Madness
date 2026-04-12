using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time")]
    [Range(0, 24)]
    public float startHour = 8f;
    public float minutesPerSecond = 1f;
   
    [Header("Sun")]
    public Light SunLight;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;
    public float maxSunIntensity= 1.5f;
    
    [Header("Ambient")]
    public Gradient ambientColor;
    
    [Header("Events")]
    public UnityEvent onSunrise;
    public UnityEvent OnMidnight;
    public UnityEvent onSunset;

    public float CurrentHour => _currentHour;
    public bool isNight => _currentHour >= 20f || _currentHour < 6f;
    public bool isDay => !isNight;

    public string TimeString
    {
        get
        {
            int hours = Mathf.FloorToInt(_currentHour);
            int minutes = Mathf.FloorToInt((_currentHour - hours) * 60f);
            return $"{hours:00}:{minutes:00}";
        }
    }
    
    private float _currentHour;
    private bool sunriseFired;
    private bool sunsetFired;
    private bool midnightFired;

    private void Awake()
    {
        Instance = this;
        _currentHour = startHour;
    }
    private void Update()
    {
        AdvanceTime();
        UpdateSun();
        CheckEvents();
    }
    private void AdvanceTime()
    {
        float minutesToAdd = minutesPerSecond * UnityEngine.Time.deltaTime;
        _currentHour += minutesToAdd / 60f;
        
        if (_currentHour >= 24f)
        {
            _currentHour -= 24f;
            ResetDailyFlag();
        }
    }

    private void UpdateSun()
    {
        if(SunLight ==null)  return;

        float t = _currentHour / 24f;
        float angel = (t * 360f) - 90f;
        SunLight.transform.rotation = Quaternion.Euler(angel, -30f, 0f);
        
        SunLight.color = sunColor.Evaluate(t);
        SunLight.intensity = sunIntensity.Evaluate(t)*maxSunIntensity;

        if (ambientColor != null)
        {
            RenderSettings.ambientLight = ambientColor.Evaluate(t);
        }
    }

    private void ResetDailyFlag()
    {
        sunriseFired = false;
        sunsetFired = false;
        midnightFired = false;
    }

    private void CheckEvents()
    {
        if (!sunriseFired && _currentHour >= 6f && _currentHour < 6.5f)
        {
            sunriseFired = true;
            onSunrise?.Invoke();
        }
        
        if (!sunsetFired && _currentHour >= 20f && _currentHour < 20.5f)
        {
            sunsetFired = true;
            onSunset?.Invoke();
        }
        
        if (!midnightFired && (_currentHour >= 0f && _currentHour < 0.5f || _currentHour >= 23.5f))
        {
            midnightFired = true;
            OnMidnight?.Invoke();
        }
    }

    public void SetHour(float hour)
    {
        _currentHour = Mathf.Clamp(hour, 0, 24f);
    }

    public void SkipHour(float hour)
    {
        _currentHour = (_currentHour + hour)%24f;
    }
    
}
