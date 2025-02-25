/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

Magnitude.cs

*/
using System;
using UnityEngine;

// TODO: add custom calculation?
// TODO: add curve calculation?
public enum MagnitudeCalculation
{
    /// <summary>
    /// Result = Value
    /// </summary>
    Simple,
    /// <summary>
    /// Result = Value + AttributeValue
    /// </summary>
    AttributeAdd,
    /// <summary>
    /// Result = Value * (1 + AttributeValue)
    /// </summary>
    AttributeAddFraction,
    /// <summary>
    /// Result = Value - AttributeValue
    /// </summary>
    AttributeSubtract,
    /// <summary>
    /// Result = Value * (1 - AttributeValue)
    /// </summary>
    AttributeSubtractFraction,
    /// <summary>
    /// Result = Value * AttributeValue
    /// </summary>
    AttributeMultiply,
    /// <summary>
    /// Result = Value / AttributeValue
    /// </summary>
    AttributeDivide,
    /// <summary>
    /// Result = AttributeValue
    /// </summary>
    AttributeOverride,
}

public enum MagnitudeAttributeProvider
{
    Source,
    Target,
}

[Serializable]
public class Magnitude
{
    public MagnitudeCalculation Calculation = MagnitudeCalculation.Simple;
    public MagnitudeAttributeProvider AttributeProvider = MagnitudeAttributeProvider.Target;
    public AttributeType Attribute;
    public float Value;

    public float Calculate(EffectContext effectContext)
    {
        return CalculateImpl(GetAttributeValue(effectContext));
    }

    public float Calculate(AbilityInstance abilityInstance)
    {
        return CalculateImpl(GetAttributeValue(abilityInstance));
    }

    private AbilitySystemComponent GetAbilitySystemComponent(EffectContext effectContext)
    {
        switch (AttributeProvider)
        {
            case MagnitudeAttributeProvider.Source:
                return effectContext.Source;
            case MagnitudeAttributeProvider.Target:
                return effectContext.Target;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private AbilitySystemComponent GetAbilitySystemComponent(AbilityInstance abilityInstance)
    {
        return abilityInstance.AbilitySystemComponent;
    }

    private float GetAttributeValue(EffectContext effectContext)
    {
        if (Calculation == MagnitudeCalculation.Simple)
            return 0;

        var value = GetAbilitySystemComponent(effectContext).GetAttributeValue(Attribute, out var attributeExists);

        if (!attributeExists)
        {
            Debug.LogAssertion($"{AttributeProvider} ASC doesn't have attribute {Attribute.GetName()}");
        }

        return value;
    }

    private float GetAttributeValue(AbilityInstance abilityInstance)
    {
        if (Calculation == MagnitudeCalculation.Simple)
            return 0;

        var value = GetAbilitySystemComponent(abilityInstance).GetAttributeValue(Attribute, out var attributeExists);

        if (!attributeExists)
        {
            Debug.LogAssertion($"Ability ASC doesn't have attribute {Attribute.GetName()}");
        }

        return value;
    }

    private float CalculateImpl(float attributeValue)
    {
        switch (Calculation)
        {
            case MagnitudeCalculation.Simple:
                return Value;
            case MagnitudeCalculation.AttributeAdd:
                return Value + attributeValue;
            case MagnitudeCalculation.AttributeAddFraction:
                return Value * (1f + attributeValue);
            case MagnitudeCalculation.AttributeSubtract:
                return Value - attributeValue;
            case MagnitudeCalculation.AttributeSubtractFraction:
                return Value * (1f - attributeValue);
            case MagnitudeCalculation.AttributeMultiply:
                return Value * attributeValue;
            case MagnitudeCalculation.AttributeDivide:
                return Value / attributeValue;
            case MagnitudeCalculation.AttributeOverride:
                return attributeValue;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
