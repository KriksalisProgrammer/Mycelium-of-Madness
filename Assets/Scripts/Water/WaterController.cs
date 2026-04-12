using UnityEngine;

public class WaterController : MonoBehaviour
{
    [Header("Материал воды")]
    public Material waterMaterial;

    [Header("Цвет воды по времени суток")]
    public Gradient waterColor;       

    [Header("Скорость волн")]
    public float dayWaveSpeed   = 0.5f;   
    public float nightWaveSpeed = 1.2f;   

    [Header("Название свойств в Shader Graph")]
    public string colorProperty     = "_WaterColor";   
    public string waveSpeedProperty = "_WaveSpeed";    

    private static readonly int ColorProp     = Shader.PropertyToID("_WaterColor");
    private static readonly int WaveSpeedProp = Shader.PropertyToID("_WaveSpeed");

    void Awake()
    {

        if (waterMaterial == null)
            waterMaterial = GetComponent<Renderer>().material;

        SetupDefaultGradient();
    }

    void Update()
    {
        if (TimeManager.Instance == null || waterMaterial == null) return;

        float t    = TimeManager.Instance.CurrentHour / 24f;
        bool night = TimeManager.Instance.isNight;

        // Цвет
        waterMaterial.SetColor(ColorProp, waterColor.Evaluate(t));
        
        float targetSpeed = night ? nightWaveSpeed : dayWaveSpeed;
        float curSpeed    = waterMaterial.GetFloat(WaveSpeedProp);
        waterMaterial.SetFloat(WaveSpeedProp, Mathf.Lerp(curSpeed, targetSpeed, Time.deltaTime));
    }

    private void SetupDefaultGradient()
    {
        if (waterColor != null) return;
        
        waterColor = new Gradient();
        var colors = new GradientColorKey[]
        {
            new(new Color(0.05f, 0.05f, 0.15f), 0.00f), 
            new(new Color(0.10f, 0.20f, 0.35f), 0.25f),  
            new(new Color(0.10f, 0.30f, 0.50f), 0.50f),  
            new(new Color(0.08f, 0.15f, 0.30f), 0.83f),  
            new(new Color(0.05f, 0.05f, 0.15f), 1.00f),  
        };
        var alphas = new GradientAlphaKey[]
        {
            new(0.85f, 0f),
            new(0.75f, 0.5f),
            new(0.85f, 1f),
        };
        waterColor.SetKeys(colors, alphas);
    }
}