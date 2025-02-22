/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

AttributeModifierStack.cs

*/

using System;
using System.Collections.Generic;
using System.Linq;
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
        UpdateModifiers();

        foreach (var modifierInstance in _modifiers)
        {
            modifierInstance.Apply(ref value);
        }

        return value;
    }

    public float CalculateWithExtraModifier(float value, ScalarModifier scalarModifier, bool post)
    {
        if (scalarModifier.IsIdentity)
            return Calculate(value);

        // if it's a post modifier, it will be applied last so shortcut is possible
        if (post)
            return scalarModifier.Calculate(Calculate(value));

        UpdateModifiers();

        if (_modifiers.Count == 0)
        {
            return scalarModifier.Calculate(value);
        }

        if (_modifiers.Last().Post)
        {
            bool applied = false;

            foreach (var modifierInstance in _modifiers)
            {
                // apply extra modifier before first post modifier
                if (!applied && modifierInstance.Post)
                {
                    value = scalarModifier.Calculate(value);
                    applied = true;
                }

                modifierInstance.Apply(ref value);
            }
        }
        else
        {
            // there are no post modifiers
            value = scalarModifier.Calculate(Calculate(value));
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
        if (_dirty)
        {
            _modifiers.Sort(CompareModifierInstances);
            _dirty = false;
        }
    }
}
