using UnityEngine;
using System;
using System.Collections.Generic;

public enum OldAttributeModifierType
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
    OldAbilitySystemComponent SourceAbilitySystem { get; }
    OldAbilitySystemComponent TargetAbilitySystem { get; }
}

public interface IAttributeModifierCalculation
{
    float GetValue(IAttributeModifierContext context, OldAttributeModifier attributeModifier);
}

public sealed class OldAttributeModifierHandle
{
    public OldAttributeModifier AttributeModifier => _weakAttributeModifier.TryGetTarget(out var attributeModifier) ? attributeModifier : null;
    public OldAttributeValue AttributeValue => _weakAttributeValue.TryGetTarget(out var attributeValue) ? attributeValue : null;
    public object InternalModifier => _weakInternalModifier.TryGetTarget(out var opaque) ? opaque : null;

    private readonly WeakReference<OldAttributeModifier> _weakAttributeModifier;
    private readonly WeakReference<OldAttributeValue> _weakAttributeValue;
    private readonly WeakReference<object> _weakInternalModifier;

    public OldAttributeModifierHandle(OldAttributeModifier attributeModifier, OldAttributeValue attributeValue, object internalModifier)
    {
        _weakAttributeModifier = new WeakReference<OldAttributeModifier>(attributeModifier);
        _weakAttributeValue = new WeakReference<OldAttributeValue>(attributeValue);
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
public sealed class OldAttributeModifier : ScriptableObject
{
    public AttributeType Attribute;
    public OldAttributeModifierType Type;
    public float Value = 0f;
    public bool Post = false;
    public bool Permanent = false;
}
