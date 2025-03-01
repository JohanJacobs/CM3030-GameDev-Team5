/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5

Please refer to the README file for detailled information

Player.cs

Class Player is used to manage some player attack related abilities

*/

using UnityEngine;

public class Player : Creature, IAttackAbilityAim, IAttackAbilityDispatcher
{
    public delegate void AttackDelegate(AbilityInstance abilityInstance, Vector3 origin, Vector3 direction, EquipmentSlot abilityEquipmentSlot);

    public event AttackDelegate CommittedAttack;

    public float Speed => AbilitySystemComponent.GetAttributeValue(AttributeType.MoveSpeed);
    public float TurnSpeed => AbilitySystemComponent.GetAttributeValue(AttributeType.TurnSpeed);
    public float Experience => AbilitySystemComponent.GetAttributeValue(AttributeType.Experience);
    public float Level => AbilitySystemComponent.GetAttributeValue(AttributeType.Level);

    private Vector3 _attackAbilityAimOrigin;
    private Vector3 _attackAbilityAimTarget;
    private Vector3 _attackAbilityAimDirection;

    public Player()
    {
        // player GO should exist even if player died
        _autoDestroyOnDeath = false;
    }

    public void UpdateAttackAbilityAim(in Vector3 origin, in Vector3 target, in Vector3 direction)
    {
        _attackAbilityAimOrigin = origin;
        _attackAbilityAimTarget = target;
        _attackAbilityAimDirection = direction;
    }

    public Vector3 GetAttackAbilityAimOrigin()
    {
        return _attackAbilityAimOrigin;
    }

    public Vector3 GetAttackAbilityAimTarget()
    {
        return _attackAbilityAimTarget;
    }

    public Vector3 GetAttackAbilityAimDirection()
    {
        return _attackAbilityAimDirection;
    }

    public void OnAttackCommitted(AbilityInstance abilityInstance, Vector3 origin, Vector3 direction, EquipmentSlot abilityEquipmentSlot)
    {
        CommittedAttack?.Invoke(abilityInstance, origin, direction, abilityEquipmentSlot);
    }
}