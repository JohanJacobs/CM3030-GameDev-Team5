using UnityEngine;
using System;
using System.Collections.Generic;

public enum AttributeModifierType
{
    /// <summary>
    /// Adds an absolute value
    /// </summary>
    Add,
    /// <summary>
    /// Adds an absolute value that depends on modifier level
    /// </summary>
    AddProgressive,
    /// <summary>
    /// Adds a fraction of base value, like +20% or -50%
    /// </summary>
    AddFraction,
    /// <summary>
    /// Adds a fraction of base value that depends on modifier level
    /// </summary>
    AddFractionProgressive,
    /// <summary>
    /// Multiplies by absolute value
    /// </summary>
    Multiply,
    /// <summary>
    /// Multiplies by absolute value that depends on modifier level
    /// </summary>
    MultiplyProgressive,
    /// <summary>
    /// Replaces base value with modifier value
    /// </summary>
    Override,
}

public interface IAttributeModifierContext
{
    GameObject Source { get; }
    GameObject Target { get; }
    AbilitySystemComponent SourceAbilitySystem { get; }
    AbilitySystemComponent TargetAbilitySystem { get; }
}

public interface IAttributeModifierCalculation
{
    float GetValue(IAttributeModifierContext context, AttributeModifier attributeModifier);
}

public sealed class AttributeModifierHandle
{
    public AttributeModifier AttributeModifier => _weakAttributeModifier.TryGetTarget(out var attributeModifier) ? attributeModifier : null;
    public AttributeValue AttributeValue => _weakAttributeValue.TryGetTarget(out var attributeValue) ? attributeValue : null;
    public object InternalModifier => _weakInternalModifier.TryGetTarget(out var opaque) ? opaque : null;

    private readonly WeakReference<AttributeModifier> _weakAttributeModifier;
    private readonly WeakReference<AttributeValue> _weakAttributeValue;
    private readonly WeakReference<object> _weakInternalModifier;

    public AttributeModifierHandle(AttributeModifier attributeModifier, AttributeValue attributeValue, object internalModifier)
    {
        _weakAttributeModifier = new WeakReference<AttributeModifier>(attributeModifier);
        _weakAttributeValue = new WeakReference<AttributeValue>(attributeValue);
        _weakInternalModifier = new WeakReference<object>(internalModifier);
    }

    public bool CancelModifier()
    {
        var attributeValue = AttributeValue;
        if (attributeValue == null)
            return false;

        attributeValue.CancelModifier(this);

        return true;
    }
}

[CreateAssetMenu]
public sealed class AttributeModifier : ScriptableObject
{
    public AttributeType Attribute;
    public AttributeModifierType Type;
    public float Value = 0f;
    public bool Post = false;
    public bool Permanent = false;
}
