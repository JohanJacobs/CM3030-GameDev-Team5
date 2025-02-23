using UnityEngine;

public class WeaponPickup : Pickup
{
    // cache reference for easier access
    private static AudioManager audioManager => AudioManager.Instance;

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
            audioManager.PlaySFX(audioManager.grabExperienceSound);
            return true;
        }

        return false;
    }
}
