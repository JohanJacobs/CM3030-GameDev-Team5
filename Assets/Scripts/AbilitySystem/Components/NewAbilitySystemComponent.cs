using System;
using System.Collections.Generic;
using UnityEngine;

public class NewAbilitySystemComponent : MonoBehaviour
{
    public delegate void SelfDelegate(NewAbilitySystemComponent asc);

    [SerializeField]
    private NewAttributeSet[] _grantedAttributeSets;

    private readonly List<NewAttributeSetInstance> _attributeSets = new List<NewAttributeSetInstance>();
    private readonly EnumArray<NewAttributeValue, AttributeType> _attributeValues = new EnumArray<NewAttributeValue, AttributeType>();

    private bool _ready;

    private event SelfDelegate Ready;

    public void OnReady(SelfDelegate callback)
    {
        if (_ready)
        {
            callback.Invoke(this);
        }
        else
        {
            Ready += callback;
        }
    }

    public void AddAttributeSet(NewAttributeSet attributeSet)
    {
        var index = _attributeSets.FindIndex(asi => asi.Template == attributeSet);
        if (index >= 0)
            return;

        var instance = attributeSet.CreateInstance();

        _attributeSets.Add(instance);

        foreach (var attributeValue in instance.AttributeValues)
        {
            var existingAttributeValue = _attributeValues[attributeValue.Attribute];
            if (existingAttributeValue != null)
                throw new InvalidOperationException($"Ambiguous attribute {attributeValue.Attribute.GetName()}");

            _attributeValues[attributeValue.Attribute] = attributeValue;
        }
    }

    public void RemoveAttributeSet(NewAttributeSet attributeSet)
    {
        var index = _attributeSets.FindIndex(asi => asi.Template == attributeSet);
        if (index < 0)
            return;

        var instance = _attributeSets[index];

        foreach (var attributeValue in instance.AttributeValues)
        {
            var existingAttributeValue = _attributeValues[attributeValue.Attribute];
            if (existingAttributeValue != attributeValue)
                throw new InvalidOperationException($"Ambiguous attribute {attributeValue.Attribute.GetName()}");

            _attributeValues[attributeValue.Attribute] = null;
        }

        _attributeSets.RemoveAt(index);
    }

    private void Start()
    {
        _ready = true;

        Ready?.Invoke(this);
    }
}
