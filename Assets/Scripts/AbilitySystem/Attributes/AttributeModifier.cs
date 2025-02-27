/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

AttributeModifier.cs

*/

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
