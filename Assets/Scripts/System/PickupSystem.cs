using UnityEngine;
using UnityEngine.Events;

public class PickupSystem : MonoBehaviour
{
    public static PickupSystem Instance { get; private set; }

    [Header("Interaction Settings")]
    public Camera playerCamera;
    public float pickupRange = 3f;
    public LayerMask pickupLayer;

    [Header("Events")]
    public UnityEvent<string, int> OnItemPickedUp = new UnityEvent<string, int>();

    private InputSystem_Actions _input;
    private IPickupable _highlighted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _input = new InputSystem_Actions();
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();

    private void Update()
    {
        HandleHighlight();

        if (_input.Player.Interact.WasPressedThisFrame())
            TryPickup();
    }

    private void HandleHighlight()
    {
        IPickupable hit = RaycastForPickup();
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

        string itemType = _highlighted.ItemType;
        int amount = _highlighted.Amount;

        _highlighted.Collect();
        _highlighted = null;

        OnItemPickedUp?.Invoke(itemType, amount);
        Debug.Log($"[PickupSystem] Picked: {amount} × {itemType}");
    }

    private IPickupable RaycastForPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            return hit.collider.GetComponent<IPickupable>();
        }
        return null;
    }
}