using System;
using System.Collections.Generic;

public class NewAttributeModifierStack
{
    public delegate void SelfDelegate(NewAttributeModifierStack attributeModifierStack);

    public AttributeType Attribute { get; }

    public event SelfDelegate Changed;

    private readonly List<NewAttributeModifierInstance> _modifiers = new List<NewAttributeModifierInstance>();

    private bool _dirty;

    public NewAttributeModifierStack(AttributeType attribute)
    {
        Attribute = attribute;
    }

    public NewAttributeModifierInstance AddModifier(NewAttributeModifier modifier)
    {
        var modifierInstance = modifier.CreateInstance();

        _modifiers.Add(modifierInstance);
        _dirty = true;

        Changed?.Invoke(this);

        return modifierInstance;
    }

    public NewAttributeModifierInstance AddModifier(ScalarModifier scalarModifier, bool post)
    {
        var modifierInstance = new NewAttributeModifierInstanceWithModifier(scalarModifier, post);

        _modifiers.Add(modifierInstance);
        _dirty = true;

        Changed?.Invoke(this);

        return modifierInstance;
    }

    public NewAttributeModifierInstance AddOverride(float scalarOverride, bool post)
    {
        var modifierInstance = new NewAttributeModifierInstanceWithOverride(scalarOverride, post);

        _modifiers.Add(modifierInstance);
        _dirty = true;

        Changed?.Invoke(this);

        return modifierInstance;
    }

    public void RemoveModifier(NewAttributeModifierInstance modifierInstance)
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

    private static int CompareModifierInstances(NewAttributeModifierInstance lhs, NewAttributeModifierInstance rhs)
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
