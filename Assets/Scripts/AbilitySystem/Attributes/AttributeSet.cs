/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

AttributeSet.cs

*/

using System;
using UnityEngine;

[Serializable]
public sealed class AttributeDefinition
{
    public AttributeType Attribute => _attribute;

    public float DefaultValue => _defaultValue;

    [SerializeField]
    private AttributeType _attribute;

    [SerializeField]
    private float _defaultValue;
}

[CreateAssetMenu]
public class AttributeSet : ScriptableObject
{
    public AttributeDefinition[] DefaultAttributes => _defaultAttributes;

    [SerializeField]
    private AttributeDefinition[] _defaultAttributes;

    public AttributeSetInstance CreateInstance()
    {
        var instance = new AttributeSetInstance(this);

        return instance;
    }
}
