using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MushroomPickup : MonoBehaviour, IPickupable
{
    [Header("Pickup Data")]
    public string itemType = "Mushroom";
    public int amount = 1;

    [Header("Highlight")]
    public Renderer meshRenderer;
    public Color highlightColor = new Color(1f, 0.85f, 0.3f);

    private Color _originalColor;
    private bool _highlighted;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        if (meshRenderer != null)
            _originalColor = meshRenderer.material.color;
    }

    public string ItemType => itemType;
    public int Amount => amount;

    public void SetHighlight(bool active)
    {
        if (_highlighted == active || meshRenderer == null) return;

        _highlighted = active;
        meshRenderer.material.color = active ? highlightColor : _originalColor;
    }

    public void Collect()
    {
        SetHighlight(false);
    
        if (MushroomSpawner.Instance != null)
            MushroomSpawner.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);     
    }
}