using UnityEngine;

public class WeaponPickup : Pickup
{
    public WeaponItem WeaponItem;

    protected override bool HandlePickUpImpl(GameObject target)
    {
        var equipmentComponent = target.GetComponent<EquipmentComponent>();
        if (equipmentComponent == null)
            return false;

        if (equipmentComponent.AddItem(WeaponItem))
        {
            // auto equip
            equipmentComponent.EquipItem(WeaponItem);
            return true;
        }

        return false;
    }
}