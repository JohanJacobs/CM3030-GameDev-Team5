/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

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

    RangeScale,
    DamageScale,
    HealingScale,
    CooldownScale,
    DurationScale,
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

    public static bool IsScaleAttribute(this AttributeType self)
    {
        switch (self)
        {
            case AttributeType.RangeScale:
            case AttributeType.DamageScale:
            case AttributeType.HealingScale:
            case AttributeType.CooldownScale:
            case AttributeType.DurationScale:
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
