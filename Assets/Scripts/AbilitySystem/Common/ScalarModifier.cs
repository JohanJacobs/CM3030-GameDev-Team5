using UnityEngine;
using System;

/// <summary>
/// Defines several simple add/multiply components to transform a scalar value. Keeps track of identity to prevent float error buildup.
/// </summary>
public struct ScalarModifier
{
    public static ScalarModifier MakeIdentity()
    {
        return new ScalarModifier()
        {
            _bonus = 0f,
            _bonusFraction = 0f,
            _multiplier = 1f,
            _identity = true,
        };
    }

    public static ScalarModifier MakeBonus(float value)
    {
        return new ScalarModifier()
        {
            _bonus = value,
            _bonusFraction = 0f,
            _multiplier = 1f,
            _identity = !(Mathf.Abs(value) > 0f),
        };
    }

    public static ScalarModifier MakeBonusFraction(float value)
    {
        return new ScalarModifier()
        {
            _bonus = 0f,
            _bonusFraction = value,
            _multiplier = 1f,
            _identity = !(Mathf.Abs(value) > 0f),
        };
    }

    public static ScalarModifier MakeMultiplier(float value)
    {
        return new ScalarModifier()
        {
            _bonus = 0f,
            _bonusFraction = 0f,
            _multiplier = value,
            _identity = !(Mathf.Abs(value - 1f) > 0f),
        };
    }

    public static ScalarModifier MakeInverse(ScalarModifier modifier)
    {
        if (modifier._identity)
            return modifier;

        if (!(Mathf.Abs(modifier._multiplier) > 0f))
            throw new ArgumentOutOfRangeException(nameof(modifier), "Multiplier is 0");

        return new ScalarModifier()
        {
            _bonus = -modifier._bonus,
            _bonusFraction = -modifier._bonusFraction,
            _multiplier = 1f / modifier._multiplier,
            _identity = false,
        };
    }

    public static ScalarModifier MakeFromAttributeModifier(AttributeModifier attributeModifier)
    {
        float bonus = 0f;
        float bonusFraction = 0f;
        float multiplier = 1f;
        bool identity = true;

        switch (attributeModifier.Type)
        {
            case NewAttributeModifierType.Add:
                if (Mathf.Abs(attributeModifier.Value) > 0f)
                {
                    bonus = attributeModifier.Value;
                    identity = false;
                }
                break;
            case NewAttributeModifierType.AddFraction:
                if (Mathf.Abs(attributeModifier.Value) > 0f)
                {
                    bonusFraction = attributeModifier.Value;
                    identity = false;
                }
                break;
            case NewAttributeModifierType.Multiply:
                if (Mathf.Abs(attributeModifier.Value - 1f) > 0f)
                {
                    multiplier = attributeModifier.Value;
                    identity = false;
                }
                break;
            case NewAttributeModifierType.Override:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return new ScalarModifier()
        {
            _bonus = bonus,
            _bonusFraction = bonusFraction,
            _multiplier = multiplier,
            _identity = identity,
        };
    }

    public bool IsIdentity => _identity;

    private float _bonus;
    private float _bonusFraction;
    private float _multiplier;
    private bool _identity;

    public ScalarModifier(ScalarModifier modifier)
    {
        _bonus = modifier._bonus;
        _bonusFraction = modifier._bonusFraction;
        _multiplier = modifier._multiplier;
        _identity = modifier._identity;
    }

    public void Clear()
    {
        _bonus = 0f;
        _bonusFraction = 0f;
        _multiplier = 1f;
        _identity = true;
    }

    public void Reset(ScalarModifier modifier)
    {
        _bonus = modifier._bonus;
        _bonusFraction = modifier._bonusFraction;
        _multiplier = modifier._multiplier;
        _identity = modifier._identity;
    }

    public void Combine(ScalarModifier modifier)
    {
        if (modifier._identity)
            return;

        _bonus += modifier._bonus;
        _bonusFraction += modifier._bonusFraction;
        _multiplier *= modifier._multiplier;
        _identity = false;
    }

    public float Calculate(float baseValue)
    {
        if (_identity)
            return baseValue;

        return baseValue * (_multiplier + _bonusFraction) + _bonus;
    }
}
