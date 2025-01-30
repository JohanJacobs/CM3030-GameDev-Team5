using UnityEngine;

public class PickupWithEffect : Pickup
{
    public Effect Effect;

    public override bool HandlePickUp(GameObject target)
    {
        var asc = target.GetComponent<AbilitySystemComponent>();
        if (asc == null)
            return false;

        asc.ApplyEffect(Effect);

        return true;
    }
}
