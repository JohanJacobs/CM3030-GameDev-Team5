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
