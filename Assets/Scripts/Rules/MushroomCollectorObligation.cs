using UnityEngine;
using TMPro;
public class MushroomCollectorObligation : MonoBehaviour, IObligationSource
{
    [Header("Responsibility: mushrooms")]
    public int requiredCount = 10;

    [Header("Raycast")]
    public Camera playerCamera;
    public float  pickupRange = 2.5f;
    public LayerMask mushroomLayer;
    
    [Header("UI")]
    public TextMeshProUGUI counterText;
    
    private InputSystem_Actions _input;
    private int _collected;
    private MushroomPickup _highlighted;
    

    public string ObligationId  => "underfed_mycelium";
    public bool   IsFulfilled   => _collected >= requiredCount;
    public void ResetForNewDay()
    {
        _collected = 0;
        UpdateUI();
    }
    private void Awake()
    {
        _input = new InputSystem_Actions();
        if (playerCamera == null)
            playerCamera = Camera.main;
    }
    private void OnEnable()
    {
        _input.Enable();
        ObligationTracker.Instance?.Register(this);
        UpdateUI();
    }

    private void OnDisable()
    {
        _input.Disable();
        ObligationTracker.Instance?.Unregister(this);
    }

    private void Update()
    {
        HandleHighlight();

        if (_input.Player.Interact.WasPressedThisFrame())
            TryPickup();
    }

    private void HandleHighlight()
    {
        MushroomPickup hit = Raycast();

        if (hit != _highlighted)
        {
            _highlighted?.SetHighlight(false);
            _highlighted = hit;
            _highlighted?.SetHighlight(true);
        }
    }

    private void TryPickup()
    {
        if (_highlighted == null) return;

        _collected += _highlighted.amount;
        _highlighted.Collect();
        _highlighted = null;

        Debug.Log($"[Mushroom] {_collected}/{requiredCount}");
        UpdateUI();
    }

    private MushroomPickup Raycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, mushroomLayer))
        {
            if (hit.collider.TryGetComponent(out MushroomPickup mushroom))
                return mushroom;
        }
        return null;
    }

    private void UpdateUI()
    {
        if (counterText != null)
            counterText.text = $"Грибы: {_collected} / {requiredCount}";
    }
}