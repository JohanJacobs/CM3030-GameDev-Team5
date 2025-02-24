using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct AttributeInfo
{
    public AttributeType Attribute;
    public float BaseValue;
    public float Value;
}

public static class AbilitySystemComponentHelper
{
    private static void AddAttributeValue(this AbilitySystemComponent self, AttributeType attribute, float amount)
    {
        var attributeValue = self.GetAttributeValueObject(attribute);
        if (attributeValue == null)
            return;

        attributeValue.HACK_SetCurrentValue(attributeValue.Value + amount);
    }

    public static void AddExperience(this AbilitySystemComponent self, float amount)
    {
        Debug.Assert(!(amount < 0), "Experience amount must be non-negative");

        if (amount > 0)
            self.AddAttributeValue(AttributeType.Experience, amount);
    }

    public static void AddDamage(this AbilitySystemComponent self, float amount)
    {
        Debug.Assert(!(amount < 0), "Damage amount must be non-negative");

        if (amount > 0)
            self.AddAttributeValue(AttributeType.Damage, amount);
    }

    public static void AddHealing(this AbilitySystemComponent self, float amount)
    {
        Debug.Assert(!(amount < 0), "Healing amount must be non-negative");

        if (amount > 0)
            self.AddAttributeValue(AttributeType.Healing, amount);
    }

    public static IEnumerable<AttributeInfo> GetModifiedAttributesInfo(this AbilitySystemComponent self, IEnumerable<AttributeModifier> modifiers)
    {
        var uniqueAttributes = modifiers
            .Select(modifier => modifier.Attribute)
            .Distinct();

        foreach (var attribute in uniqueAttributes)
        {
            var attributeValue = self.GetAttributeValueObject(attribute);
            if (attributeValue == null)
                continue;

            yield return new AttributeInfo()
            {
                Attribute = attribute,
                BaseValue = attributeValue.BaseValue,
                Value = attributeValue.Value,
            };
        }
    }
}