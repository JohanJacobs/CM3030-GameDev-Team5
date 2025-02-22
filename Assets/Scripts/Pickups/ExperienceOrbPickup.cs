/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

ExperienceOrbPickup.cs

*/

using UnityEngine;

public class ExperienceOrbPickup : Pickup
{
    public float Experience;

    protected override bool HandlePickUpImpl(GameObject target)
    {
        var asc = target.GetComponent<AbilitySystemComponent>();
        if (asc == null)
            return false;

        asc.AddExperience(Experience);

        return true;
    }
}
