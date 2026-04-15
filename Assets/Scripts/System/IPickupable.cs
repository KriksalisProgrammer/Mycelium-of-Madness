using UnityEngine;

public interface IPickupable
{
    string ItemType { get; }           
    int Amount { get; }
    void Collect();                    
    void SetHighlight(bool active);
}