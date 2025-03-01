/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

AttributeModifierInstance.cs

*/

using System;
using UnityEngine;

public abstract class AttributeModifierInstance
{
    private static int _nextIndex = 0;

    public bool Post { get; }
    public int Index { get; } = ++_nextIndex;

    protected AttributeModifierInstance(AttributeModifier attributeModifier)
    {
        Post = attributeModifier.Post;
    }

    protected AttributeModifierInstance(bool post)
    {
        Post = post;
    }

    public abstract void Apply(ref float value);

    public virtual void SetAbsoluteValueChange(float change)
    {

    }
}

public sealed class AttributeModifierInstanceWithModifier : AttributeModifierInstance
{
    public float AbsoluteValueChange { get; private set; }

    private ScalarModifier _scalarModifier;

    public AttributeModifierInstanceWithModifier(AttributeModifier attributeModifier)
        : base(attributeModifier)
    {
        Debug.Assert(attributeModifier.Type != NewAttributeModifierType.Override);

        _scalarModifier = ScalarModifier.MakeFromAttributeModifier(attributeModifier);
    }

    public AttributeModifierInstanceWithModifier(ScalarModifier scalarModifier, bool post)
        : base(post)
    {
        _scalarModifier = scalarModifier;
    }

    public override void Apply(ref float value)
    {
        value = _scalarModifier.Calculate(value);
    }

    public override void SetAbsoluteValueChange(float change)
    {
        AbsoluteValueChange = change;
    }
}

public sealed class AttributeModifierInstanceWithOverride : AttributeModifierInstance
{
    private float _scalarOverride;

    public AttributeModifierInstanceWithOverride(AttributeModifier attributeModifier)
        : base(attributeModifier)
    {
        Debug.Assert(attributeModifier.Type == NewAttributeModifierType.Override);

        _scalarOverride = attributeModifier.Value;
    }

    public AttributeModifierInstanceWithOverride(float scalarOverride, bool post)
        : base(post)
    {
        _scalarOverride = scalarOverride;
    }

    public override void Apply(ref float value)
    {
        value = _scalarOverride;
    }
}

public static class AttributeModifierInstanceHelper
{
    public static float GetAbsoluteValueChange(this AttributeModifierInstance self)
    {
        switch (self)
        {
            case AttributeModifierInstanceWithModifier withModifier:
                return withModifier.AbsoluteValueChange;
            case AttributeModifierInstanceWithOverride withOverride:
                // NOTE: intended for attribute modifiers info collection, might not work for other usages
                return 0;
            default:
                throw new ArgumentOutOfRangeException(nameof(self));
        }
    }
}
