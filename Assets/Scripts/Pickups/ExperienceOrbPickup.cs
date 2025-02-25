/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

ExperienceOrbPickup.cs

*/

using UnityEngine;

public class ExperienceOrbPickup : Pickup
{
    // cache reference for easier access
    private static AudioManager audioManager => AudioManager.Instance;
    public float Experience;

    protected override bool HandlePickUpImpl(GameObject target)
    {
        var asc = target.GetComponent<AbilitySystemComponent>();
        if (asc == null)
            return false;
        audioManager.PlaySFX(audioManager.grabExperienceSound);
        asc.AddExperience(Experience);

        return true;
    }
}
