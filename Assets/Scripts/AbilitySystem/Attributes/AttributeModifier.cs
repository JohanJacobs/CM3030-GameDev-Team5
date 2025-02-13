using System;
using UnityEngine;

public enum NewAttributeModifierType
{
    Add,
    AddFraction,
    Multiply,
    Override,
}

[Serializable]
public struct AttributeModifier
{
    public AttributeType Attribute;
    public NewAttributeModifierType Type;
    public float Value;
    public bool Permanent;
    public bool Post;

    public AttributeModifierInstance CreateInstance()
    {
        AttributeModifierInstance instance;

        if (Type == NewAttributeModifierType.Override)
        {
            instance = new AttributeModifierInstanceWithOverride(this);
        }
        else
        {
            instance = new AttributeModifierInstanceWithModifier(this);
        }

        return instance;
    }
}
