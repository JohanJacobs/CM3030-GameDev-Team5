using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public sealed class AttributeDefinition
{
    public AttributeType Attribute;
    public float BaseValue;
}

[CreateAssetMenu]
public sealed class AttributeSet : ScriptableObject
{
    private static readonly AttributeType[] AllAttributes = Enum.GetValues(typeof(AttributeType)).Cast<AttributeType>().ToArray();

    /// <summary>
    /// Defines attributes in this set and their base values
    /// </summary>
    /// <remarks>
    /// Note that no other attributes can be added to the set later
    /// </remarks>
    public AttributeDefinition[] DefaultAttributes;

    public AttributeSet Template { get; private set; }

    public bool IsTemplate => Template == null;

    public IEnumerable<AttributeType> Attributes => AttributeValues.Select(attributeValue => attributeValue.Attribute);

    public IEnumerable<AttributeValue> AttributeValues => _attributeValues.Where(attributeValue => attributeValue != null);

    private readonly EnumArray<AttributeValue, AttributeType> _attributeValues = new EnumArray<AttributeValue, AttributeType>();

    /// <summary>
    /// Create and initialize a new attribute set using this set as template
    /// </summary>
    /// <remarks>
    /// If this attribute set is an instance of template, its Template will be used as template instead
    /// </remarks>
    /// <returns>Newly created attribute set</returns>
    public AttributeSet Spawn()
    {
        var attributeSet = CreateInstance<AttributeSet>();

        attributeSet.Template = IsTemplate ? this : Template;
        attributeSet.Initialize();

        return attributeSet;
    }

    /// <summary>
    /// Resets all attributes to their default values as defined by template's DefaultAttributes
    /// </summary>
    public void Initialize()
    {
        Debug.Assert(!IsTemplate, "Template attribute set must not be used directly");

        _attributeValues.Fill(null);

        foreach (var attributeDefinition in GetDefaultAttributes())
        {
            var attributeValue = _attributeValues[attributeDefinition.Attribute];
            if (attributeValue == null)
            {
                attributeValue = new AttributeValue(attributeDefinition.Attribute, this);

                _attributeValues[attributeValue.Attribute] = attributeValue;
            }
            else
            {
                Debug.LogWarning($"Found duplicate {attributeDefinition.Attribute.GetName()} attribute definition, check attribute set");
            }

            attributeValue.Base = attributeDefinition.BaseValue;
        }
    }

    public AttributeValue GetAttributeValueObject(AttributeType key)
    {
        return _attributeValues[key];
    }

    public float this[AttributeType key] => _attributeValues[key]?.Value ?? 0f;

    private AttributeDefinition[] GetDefaultAttributes()
    {
        return IsTemplate ? DefaultAttributes : Template.GetDefaultAttributes();
    }
}
