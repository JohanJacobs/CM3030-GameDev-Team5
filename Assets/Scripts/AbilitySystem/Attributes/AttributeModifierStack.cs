using System;
using System.Collections.Generic;
using UnityEngine;

public class AttributeModifierStack
{
    public delegate void SelfDelegate(AttributeModifierStack attributeModifierStack);

    public AttributeType Attribute { get; }

    public event SelfDelegate Changed;

    private readonly List<AttributeModifierInstance> _modifiers = new List<AttributeModifierInstance>();

    private bool _dirty;

    public AttributeModifierStack(AttributeType attribute)
    {
        Attribute = attribute;
    }

    public AttributeModifierInstance AddModifier(AttributeModifier attributeModifier)
    {
        Attribute.EnsureCanHaveModifiers();

        Debug.Assert(attributeModifier.Attribute == Attribute);

        var modifierInstance = attributeModifier.CreateInstance();

        _modifiers.Add(modifierInstance);
        _dirty = true;

        Changed?.Invoke(this);

        return modifierInstance;
    }

    public AttributeModifierInstance AddModifier(ScalarModifier scalarModifier, bool post)
    {
        Attribute.EnsureCanHaveModifiers();

        var modifierInstance = new AttributeModifierInstanceWithModifier(scalarModifier, post);

        _modifiers.Add(modifierInstance);
        _dirty = true;

        Changed?.Invoke(this);

        return modifierInstance;
    }

    public AttributeModifierInstance AddOverride(float scalarOverride, bool post)
    {
        Attribute.EnsureCanHaveModifiers();

        var modifierInstance = new AttributeModifierInstanceWithOverride(scalarOverride, post);

        _modifiers.Add(modifierInstance);
        _dirty = true;

        Changed?.Invoke(this);

        return modifierInstance;
    }

    public void RemoveModifier(AttributeModifierInstance modifierInstance)
    {
        if (_modifiers.Remove(modifierInstance))
        {
            _dirty = true;

            Changed?.Invoke(this);
        }
    }

    public void Reset()
    {
        _modifiers.Clear();
        _dirty = false;
    }

    public float Calculate(float value)
    {
        if (_dirty)
        {
            _dirty = false;

            UpdateModifiers();
        }

        foreach (var modifierInstance in _modifiers)
        {
            modifierInstance.Apply(ref value);
        }

        return value;
    }

    private static int CompareModifierInstances(AttributeModifierInstance lhs, AttributeModifierInstance rhs)
    {
        if (lhs.Post != rhs.Post)
            return lhs.Post ? 1 : -1;

        return lhs.Index - rhs.Index;
    }

    private void UpdateModifiers()
    {
        _modifiers.Sort(CompareModifierInstances);
    }
}
