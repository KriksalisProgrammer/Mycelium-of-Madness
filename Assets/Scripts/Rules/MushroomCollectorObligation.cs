using UnityEngine;
using TMPro;

public class MushroomCollectorObligation : BaseObligation
{
    [Header("Mushroom Specific")]
    public int mushroomsPerPickup = 1;

    [Header("UI")]
    public TextMeshProUGUI counterText;

    private void OnEnable()
    {
        ObligationTracker.Instance?.Register(this);

        // Надёжная подписка
        if (PickupSystem.Instance != null)
        {
            PickupSystem.Instance.OnItemPickedUp.AddListener(OnItemPickedUp);
            Debug.Log("[MushroomObligation] Subscribed to PickupSystem");
        }
        else
        {
            Debug.LogWarning("[MushroomObligation] PickupSystem.Instance is null!");
        }

        UpdateUI();
    }

    private void OnDisable()
    {
        ObligationTracker.Instance?.Unregister(this);

        if (PickupSystem.Instance != null)
            PickupSystem.Instance.OnItemPickedUp.RemoveListener(OnItemPickedUp);
    }

    private void OnItemPickedUp(string itemType, int amount)
    {
        if (itemType.Equals("Mushroom", System.StringComparison.OrdinalIgnoreCase))
        {
            AddProgress(amount * mushroomsPerPickup);
            Debug.Log($"[MushroomObligation] Progress updated: {currentProgress}/{requiredCount}");
        }
    }

    protected override void OnProgressChanged()
    {
        base.OnProgressChanged();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (counterText != null)
            counterText.text = $"Грибы: {currentProgress} / {requiredCount}";
    }
}