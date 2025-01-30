using UnityEngine;
using System;

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
}

public static class AttributeTypeHelper
{
    public static string GetName(this AttributeType self)
    {
        return Enum.GetName(typeof(AttributeType), self);
    }
}
