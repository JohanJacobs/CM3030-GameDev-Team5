using UnityEngine;
using System;
using System.Collections.Generic;

public class AbilitySystemComponent : MonoBehaviour
{
    public delegate void SelfDelegate(AbilitySystemComponent asc);

    private class EffectApplication
    {
        public Effect Effect;
        public float Timer = 0f;
        public uint Stacks = 0;
    }

    public AttributeSet[] DefaultAttributeSets;

    public event SelfDelegate Ready;

    private readonly List<AttributeSet> _attributeSets = new List<AttributeSet>();
    private readonly EnumArray<AttributeValue, AttributeType> _attributeValues = new EnumArray<AttributeValue, AttributeType>();
    private readonly List<EffectApplication> _effectApplications = new List<EffectApplication>();

    public void AddAttributeSet(AttributeSet template)
    {
        var index = _attributeSets.FindIndex(attributeSet => attributeSet.Template == template);
        if (index >= 0)
            return;

        var newAttributeSet = template.Spawn();

        foreach (var attribute in newAttributeSet.Attributes)
        {
            var attributeValue = _attributeValues[attribute];
            if (attributeValue == null)
                continue;

            throw new InvalidOperationException($"Ambiguous attribute {attribute.GetName()}");
        }

        foreach (var attribute in newAttributeSet.Attributes)
        {
            var attributeValue = newAttributeSet.GetAttributeValueObject(attribute);

            _attributeValues[attribute] = attributeValue;
        }

        _attributeSets.Add(newAttributeSet);
    }

    public void RemoveAttributeSet(AttributeSet template)
    {
        var index = _attributeSets.FindIndex(attributeSet => attributeSet.Template == template);
        if (index < 0)
            return;

        var oldAttributeSet = _attributeSets[index];

        foreach (var attribute in oldAttributeSet.Attributes)
        {
            _attributeValues[attribute] = null;
        }

        _attributeSets.RemoveAt(index);
    }

    public AttributeModifierHandle ApplyAttributeModifier(AttributeModifier attributeModifer)
    {
        var attributeValue = GetAttributeValueObject(attributeModifer.Attribute);
        if (attributeValue == null)
            throw new InvalidOperationException($"Attribute {attributeModifer.Attribute.GetName()} does not exist");

        return attributeValue.ApplyModifier(attributeModifer);
    }

    public AttributeModifierHandle ApplyAttributeModifier(AttributeType attribute, ScalarModifier modifier, bool post = false, bool permanent = false)
    {
        var attributeValue = GetAttributeValueObject(attribute);
        if (attributeValue == null)
            throw new InvalidOperationException($"Attribute {attribute.GetName()} does not exist");

        return attributeValue.ApplyModifier(modifier, post, permanent);
    }

    public EffectHandle ApplyEffect(Effect effect)
    {
        return null;
    }

    public AttributeSet GetAttributeValueContainer(AttributeType attribute)
    {
        return _attributeValues[attribute]?.Owner;
    }

    public AttributeValue GetAttributeValueObject(AttributeType attribute)
    {
        return _attributeValues[attribute];
    }

    public float GetAttributeValue(AttributeType attribute)
    {
        return _attributeValues[attribute]?.Value ?? 0f;
    }

    private void Start()
    {
        foreach (var attributeSet in DefaultAttributeSets)
        {
            AddAttributeSet(attributeSet);
        }

        Ready?.Invoke(this);
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;

        foreach (var effectApplication in _effectApplications)
        {
        }
    }
}
