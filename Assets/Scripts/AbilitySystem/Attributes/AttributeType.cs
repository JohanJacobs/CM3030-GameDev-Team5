/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

AttributeType.cs

*/

using System;
using UnityEngine;

// NOTE: either add new members at the end or check attribute sets for consistency
public enum AttributeType
{
    Health,
    MaxHealth,
    HealthRegeneration,
    Shield,
    MaxShield,
    ShieldRegeneration,

    MoveSpeed,
    TurnSpeed,

    MinDamage,
    MaxDamage,
    AttackRange,
    AttackRate,
    CriticalDamageChance,
    CriticalDamageMultiplier,

    Level,
    Experience,
    Damage,
    Healing,

    KnockBack,
    KnockBackResistance,

    AreaOfEffectBonus,
    AreaOfEffectBonusFraction,
    AreaOfEffectMultiplier,

    GarlicAttackRange,
    GarlicAttackRate,
    GarlicAttackDamage,

    AbsorptionRadius,
}

public static class AttributeTypeHelper
{
    public static string GetName(this AttributeType self)
    {
        return Enum.GetName(typeof(AttributeType), self);
    }

    public static bool IsMetaAttribute(this AttributeType self)
    {
        switch (self)
        {
            case AttributeType.Level:
            case AttributeType.Experience:
            case AttributeType.Damage:
            case AttributeType.Healing:
                return true;
            default:
                return false;
        }
    }

    public static void EnsureCanHaveModifiers(this AttributeType self)
    {
        Debug.Assert(!self.IsMetaAttribute(), $"Meta attribute {self.GetName()} can't have modifiers");
    }
}
