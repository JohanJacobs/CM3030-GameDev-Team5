/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

AttributeSetInstance.cs

*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttributeSetInstance
{
    public AttributeSet Template { get; }
    public IEnumerable<AttributeType> Attributes => AttributeValues.Select(attributeValue => attributeValue.Attribute);
    public IEnumerable<AttributeValue> AttributeValues => _attributeValues.Where(attributeValue => attributeValue != null);

    private readonly EnumArray<AttributeValue, AttributeType> _attributeValues = new EnumArray<AttributeValue, AttributeType>();

    public AttributeSetInstance(AttributeSet template)
    {
        Template = template;

        Initialize();
    }

    private void Initialize()
    {
        foreach (var attributeDefinition in Template.DefaultAttributes)
        {
            var attributeValue = _attributeValues[attributeDefinition.Attribute];
            if (attributeValue != null)
            {
                Debug.LogWarning($"Found duplicate {attributeDefinition.Attribute.GetName()} attribute definition, check attribute set");
                continue;
            }

            attributeValue = new AttributeValue(attributeDefinition.Attribute, attributeDefinition.DefaultValue);

            _attributeValues[attributeDefinition.Attribute] = attributeValue;
        }
    }
}
