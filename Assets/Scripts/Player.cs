using UnityEngine;

public class Player : Creature, IAttackAbilityAimProvider
{
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
}
