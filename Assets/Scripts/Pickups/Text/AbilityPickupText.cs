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

public class AbilityPickupText : PickupText
{
    protected override void UpdatePickupText(Pickup pickup, TextMeshPro tmp)
    {
        if (pickup is PickupWithEffect pickupWithEffect)
        {
            if (pickupWithEffect.Effect is GrantAbilityEffect grantAbilityEffect)
            {
                SetText(tmp, grantAbilityEffect.GrantedAbility.DisplayName, grantAbilityEffect.GrantedAbility.Description);
                return;
            }

            // TODO: add more effect types?
        }
        else
        {
            Debug.LogWarning($"Expected PickupWithEffect, got {pickup.GetType()}");
        }
    }
}