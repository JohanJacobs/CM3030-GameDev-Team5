using System;
using System.Linq;
using UnityEngine;

public interface IAttackAbilityAimProvider
{
    Vector3 GetAttackOrigin();
    Vector3 GetAttackTarget();
    Vector3 GetAttackDirection();
}

[AbilityInstanceDataClass]
public class AttackAbilityInstanceData : AbilityInstanceData
{
    public EquipmentItem EquipmentItem;
    public EquipmentSlot EquipmentSlot;
    public bool ProvidedByEquipment;
}

public abstract class AttackAbility : Ability
{
    public Magnitude DamageMin;
    public Magnitude DamageMax;
    public Magnitude Range;

    protected AttackAbility()
    {
        AbilityInstanceDataClass = new AbilityInstanceDataClass(typeof(AttackAbilityInstanceData));
    }

    protected bool GetEquipmentAttackOrigin(AbilityInstance abilityInstance, out Vector3 origin)
    {
        var owner = abilityInstance.Owner;
        var data = abilityInstance.GetData<AttackAbilityInstanceData>();

        if (data.ProvidedByEquipment)
        {
            if (data.EquipmentItem is WeaponEquipmentItem weaponEquipmentItem)
            {
                origin = weaponEquipmentItem.MuzzleTransform.position;
                return true;
            }

            var attachmentSlot = FindEquipmentAttachmentSlot(owner, data.EquipmentSlot);
            if (attachmentSlot != null)
            {
                origin = attachmentSlot.Socket.position;
                return true;
            }
        }

        origin = Vector3.zero;
        return false;
    }

    protected void GetDefaultAttackOriginAndDirection(AbilityInstance abilityInstance, out Vector3 origin, out Vector3 direction)
    {
        Vector3 attackOrigin;
        Vector3 attackDirection;

        bool wantsAttackOrigin = !GetEquipmentAttackOrigin(abilityInstance, out attackOrigin);

        var owner = abilityInstance.Owner;

        if (owner is IAttackAbilityAimProvider aimProvider)
        {
            if (wantsAttackOrigin)
                attackOrigin = aimProvider.GetAttackOrigin();
            attackDirection = aimProvider.GetAttackDirection();
        }
        else
        {
            if (wantsAttackOrigin)
                attackOrigin = owner.transform.position;
            attackDirection = owner.transform.forward;
        }

        attackDirection.y = 0;
        attackDirection.Normalize();

        origin = attackOrigin;
        direction = attackDirection;
    }

    private EquipmentAttachmentSlot FindEquipmentAttachmentSlot(Creature creature, EquipmentSlot equipmentSlot)
    {
        return creature.EquipmentAttachmentSlots.FirstOrDefault(s => s.Slot == equipmentSlot);
    }
}