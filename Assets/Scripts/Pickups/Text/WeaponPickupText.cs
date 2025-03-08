/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

Please view README file for detailed information

*/
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