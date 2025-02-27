﻿/*

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

public class Player : Creature, IAttackAbilityAimProvider, IAttackAbilityDispatcher
{
    public delegate void AttackDelegate(AbilityInstance abilityInstance, Vector3 origin, Vector3 direction, float damage);

    public event AttackDelegate CommittedAttack;

    public float Speed => AbilitySystemComponent.GetAttributeValue(AttributeType.MoveSpeed);
    public float TurnSpeed => AbilitySystemComponent.GetAttributeValue(AttributeType.TurnSpeed);
    public float DamageMin => AbilitySystemComponent.GetAttributeValue(AttributeType.MinDamage);
    public float DamageMax => AbilitySystemComponent.GetAttributeValue(AttributeType.MaxDamage);
    public float FireRate => AbilitySystemComponent.GetAttributeValue(AttributeType.AttackRate);
    public float FireRange => AbilitySystemComponent.GetAttributeValue(AttributeType.AttackRange);
    public float Experience => AbilitySystemComponent.GetAttributeValue(AttributeType.Experience);
    public float Level => AbilitySystemComponent.GetAttributeValue(AttributeType.Level);

    private Vector3 _attackOrigin;
    private Vector3 _attackTarget;
    private Vector3 _attackDirection;

    public Player()
    {
        // player GO should exist even if player died
        _autoDestroyOnDeath = false;
    }

    public void UpdateAttackAim(in Vector3 origin, in Vector3 target, in Vector3 direction)
    {
        _attackOrigin = origin;
        _attackTarget = target;
        _attackDirection = direction;
    }

    public Vector3 GetAttackOrigin()
    {
        return _attackOrigin;
    }

    public Vector3 GetAttackTarget()
    {
        return _attackTarget;
    }

    public Vector3 GetAttackDirection()
    {
        return _attackDirection;
    }

    public void OnAttackCommitted(AbilityInstance abilityInstance, Vector3 origin, Vector3 direction, float damage)
    {
        CommittedAttack?.Invoke(abilityInstance, origin, direction, damage);
    }
}
