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

[CreateAssetMenu]
public sealed class AttributeModifier : ScriptableObject
{
    public AttributeType Attribute;
    public AttributeModifierType Type;
    public float Value = 0f;
    public bool Post = false;
    public bool Permanent = false;
}
