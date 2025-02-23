using TMPro;
using UnityEngine;

public class WeaponPickupText : PickupText
{
    protected override void UpdatePickupText(Pickup pickup, TextMeshPro tmp)
    {
        if (pickup is WeaponPickup weaponPickup)
        {
            SetText(tmp, weaponPickup.WeaponItem.DisplayName, string.Empty);
        }
        else
        {
            Debug.LogWarning($"Expected WeaponPickup, got {pickup.GetType()}");
        }
    }
}