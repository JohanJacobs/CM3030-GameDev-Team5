/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

PickupWithEffect.cs

*/

using UnityEngine;

public class PickupWithEffect : Pickup
{
    public Effect Effect;

    protected override bool HandlePickUpImpl(GameObject target)
    {
        var asc = target.GetComponent<AbilitySystemComponent>();
        if (asc == null)
            return false;

        asc.ApplyEffectToSelf(Effect);

        return true;
    }
}
