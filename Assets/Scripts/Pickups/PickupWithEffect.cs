using UnityEngine;

public class PickupWithEffect : Pickup
{
    // cache reference for easier access
    private static AudioManager audioManager => AudioManager.Instance;

    public Effect Effect;

    protected override bool HandlePickUpImpl(GameObject target)
    {
        var asc = target.GetComponent<AbilitySystemComponent>();
        if (asc == null)
            return false;

        audioManager.PlaySFX(audioManager.grabExperienceSound);
        asc.ApplyEffectToSelf(Effect);

        return true;
    }
}
