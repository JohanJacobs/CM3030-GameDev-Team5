using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewAttributeSetInstance
{
    public NewAttributeSet Template { get; }
    public IEnumerable<AttributeType> Attributes => AttributeValues.Select(attributeValue => attributeValue.Attribute);
    public IEnumerable<NewAttributeValue> AttributeValues => _attributeValues.Where(attributeValue => attributeValue != null);

    private readonly EnumArray<NewAttributeValue, AttributeType> _attributeValues = new EnumArray<NewAttributeValue, AttributeType>();

    public NewAttributeSetInstance(NewAttributeSet template)
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

            attributeValue = new NewAttributeValue(this, attributeDefinition.Attribute, attributeDefinition.DefaultValue);

            _attributeValues[attributeDefinition.Attribute] = attributeValue;
        }
    }
}
