using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private float timeScale = 60f;        
    [SerializeField] private int startHour = 8;
    public float GameTime { get; private set; }            
    public int CurrentDay { get; private set; } = 1;
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }
    public bool IsNight => CurrentHour >= 22 || CurrentHour <= 5;
    
    public enum InfectionPhase { Early, Advanced, Critical }
    public InfectionPhase CurrentPhase { get; private set; } = InfectionPhase.Early;

    public UnityEvent OnMinutePassed = new();
    public UnityEvent OnHourPassed = new();
    public UnityEvent OnDayPassed = new();
    public UnityEvent OnNightStart = new();
    public UnityEvent OnDayStart = new();
    public UnityEvent<InfectionPhase> OnPhaseChanged = new();
    
    private float timer;
    private int lastMinute = -1;
    private int lastHour = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CurrentHour = startHour;
        
    }
    private void Update()
    {
        timer += Time.deltaTime * timeScale;
        GameTime += Time.deltaTime * timeScale;

        int totalMinutes = Mathf.FloorToInt(GameTime / 60f);
        CurrentMinute = totalMinutes % 60;
        CurrentHour = (startHour + totalMinutes / 60) % 24;

        if (CurrentHour / 24 > CurrentDay - 1)
        {
            CurrentDay++;
            OnDayPassed?.Invoke();
        }

        CheckPhaseChange();
        CheckTimeEvents();
    }
    private void CheckTimeEvents()
    {
        if (CurrentMinute != lastMinute)
        {
            lastMinute = CurrentMinute;
            OnMinutePassed?.Invoke();
        }

        if (CurrentHour != lastHour)
        {
            lastHour = CurrentHour;
            OnHourPassed?.Invoke();

            if (CurrentHour == 22) OnNightStart?.Invoke();
            if (CurrentHour == 6) OnDayStart?.Invoke();
        }
    }

    private void CheckPhaseChange()
    {
        InfectionPhase newPhase = CurrentPhase;

        if (GameTime > 1800) newPhase = InfectionPhase.Advanced;     
        if (GameTime > 3600) newPhase = InfectionPhase.Critical;      

        if (newPhase != CurrentPhase)
        {
            CurrentPhase = newPhase;
            OnPhaseChanged?.Invoke(CurrentPhase);
            
        }
    }

    public void SetTimeScale(float newScale) => timeScale = newScale;
    public float GetRealTimePassed() => GameTime / 60f;
    
}
