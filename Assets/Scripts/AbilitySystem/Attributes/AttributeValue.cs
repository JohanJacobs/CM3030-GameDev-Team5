/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

AttributeValue.cs

*/

using System;
using UnityEngine;

public class AttributeValue
{
    public delegate void ValueDelegate(AttributeValue attributeValue, float oldValue, float newValue);

    public AttributeType Attribute { get; }

    public float BaseValue => _baseValue;
    public float Value => _currentValue;

    public event ValueDelegate BaseValueChanged;
    public event ValueDelegate ValueChanged;

    private float _baseValue;
    private float _currentValue;

    public AttributeValue(AttributeType attribute)
    {
        Attribute = attribute;
    }

    public AttributeValue(AttributeType attribute, float defaultValue)
        : this(attribute)
    {
        _baseValue = defaultValue;
        _currentValue = defaultValue;
    }

    public void Reset(float value)
    {
        _baseValue = value;
        _currentValue = value;
    }

    // TEMP for further design changes
    public void HACK_SetBaseValue(float value)
    {
        SetBaseValue(value);
    }

    // TEMP for further design changes
    public void HACK_SetCurrentValue(float value)
    {
        SetCurrentValue(value);
    }

    private void SetBaseValue(float value)
    {
        if (Mathf.Approximately(_baseValue, value))
            return;

        var oldValue = _baseValue;

        _baseValue = value;

        BaseValueChanged?.Invoke(this, oldValue, value);
    }

    private void SetCurrentValue(float value)
    {
        if (Mathf.Approximately(_currentValue, value))
            return;

        var oldValue = _currentValue;

        _currentValue = value;

        ValueChanged?.Invoke(this, oldValue, value);
    }
}
