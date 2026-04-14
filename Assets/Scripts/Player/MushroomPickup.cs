using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MushroomPickup: MonoBehaviour
{
    public string mushroomName = "Common mushroom";
    public int amount = 1; 
    
    [Header("Highlight")]
    public Renderer meshRenderer;
    public Color highlightColor = new Color(1f, 0.85f, 0.3f);
    
    private Color _originalColor;
    private bool  _highlighted;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    
    private void Awake()
    {
        if (meshRenderer != null)
            _originalColor = meshRenderer.material.color;
    }
    
    public void SetHighlight(bool on)
    {
        if (_highlighted == on || meshRenderer == null) return;
        _highlighted = on;
        meshRenderer.material.color = on ? highlightColor : _originalColor;
    }

    public void Collect()
    {
        SetHighlight(false);
        Destroy(gameObject);
    }
}
