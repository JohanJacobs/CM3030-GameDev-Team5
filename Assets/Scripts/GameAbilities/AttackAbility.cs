/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

AttackAbilityInstanceData.cs

*/

using System;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public interface IAttackAbilityAim
{
    Vector3 GetAttackAbilityAimOrigin();
    Vector3 GetAttackAbilityAimTarget();
    Vector3 GetAttackAbilityAimDirection();
}

public interface IAttackAbilityDispatcher
{
    void OnAttackCommitted(AbilityInstance abilityInstance, Vector3 origin, Vector3 direction, float damage, EquipmentSlot abilityEquipmentSlot);
}

[AbilityInstanceDataClass]
public class AttackAbilityInstanceData : AbilityInstanceData
{
    public EquipmentItem EquipmentItem;
    public EquipmentSlot EquipmentSlot = EquipmentSlot.Undefined;

    public bool ProvidedByEquipment => EquipmentSlot != EquipmentSlot.Undefined;
}

public abstract class AttackAbility : Ability
{
    public DamageEffect DamageEffect;

    public Magnitude DamageMin;
    public Magnitude DamageMax;
    public Magnitude Range;

    protected AttackAbility()
    {
        AbilityInstanceDataClass = new AbilityInstanceDataClass(typeof(AttackAbilityInstanceData));
    }

    protected bool GetEquipmentAim(AbilityInstance abilityInstance, out Vector3 origin)
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

            if (owner.TryFindEquipmentAttachmentSlot(data.EquipmentSlot, out var attachmentSlot))
            {
                origin = attachmentSlot.Socket.position;
                return true;
            }
        }

        origin = Vector3.zero;
        return false;
    }

    protected bool GetEquipmentAim(AbilityInstance abilityInstance, out Vector3 origin, out Vector3 direction)
    {
        var owner = abilityInstance.Owner;

        var data = abilityInstance.GetData<AttackAbilityInstanceData>();

        if (data.ProvidedByEquipment)
        {
            if (data.EquipmentItem is WeaponEquipmentItem weaponEquipmentItem)
            {
                origin = weaponEquipmentItem.MuzzleTransform.position;
                direction = weaponEquipmentItem.MuzzleTransform.forward;
                return true;
            }

            if (owner.TryFindEquipmentAttachmentSlot(data.EquipmentSlot, out var attachmentSlot))
            {
                origin = attachmentSlot.Socket.position;
                direction = attachmentSlot.Socket.forward;
                return true;
            }
        }

        origin = Vector3.zero;
        direction = Vector3.forward;
        return false;
    }

    protected bool GetEquipmentAimWithOwnerDirection(AbilityInstance abilityInstance, out Vector3 origin, out Vector3 direction)
    {
        GetOwnerAim(abilityInstance, out var ownerOrigin, out direction);

        var owner = abilityInstance.Owner;

        var data = abilityInstance.GetData<AttackAbilityInstanceData>();

        if (data.ProvidedByEquipment)
        {
            if (data.EquipmentItem is WeaponEquipmentItem weaponEquipmentItem)
            {
                origin = weaponEquipmentItem.MuzzleTransform.position;
                return true;
            }

            if (owner.TryFindEquipmentAttachmentSlot(data.EquipmentSlot, out var attachmentSlot))
            {
                origin = attachmentSlot.Socket.position;
                return true;
            }
        }

        origin = ownerOrigin;
        return false;
    }

    protected void GetOwnerAim(AbilityInstance abilityInstance, out Vector3 origin, out Vector3 direction)
    {
        Vector3 attackOrigin;
        Vector3 attackDirection;

        var owner = abilityInstance.Owner;

        switch (owner)
        {
            case IAttackAbilityAim aim:
                attackOrigin = aim.GetAttackAbilityAimOrigin();
                attackDirection = aim.GetAttackAbilityAimDirection();
                break;
            default:
                attackOrigin = owner.transform.position;
                attackDirection = owner.transform.forward;
                break;
        }

        attackDirection.y = 0;
        attackDirection.Normalize();

        origin = attackOrigin;
        direction = attackDirection;
    }

    protected void NotifyAttackCommitted(AbilityInstance abilityInstance, Vector3 origin, Vector3 direction, float damage)
    {
        var owner = abilityInstance.Owner;

        var data = abilityInstance.GetData<AttackAbilityInstanceData>();

        if (owner is IAttackAbilityDispatcher dispatcher)
        {
            dispatcher.OnAttackCommitted(abilityInstance, origin, direction, damage, data.EquipmentSlot);
        }
    }
}
