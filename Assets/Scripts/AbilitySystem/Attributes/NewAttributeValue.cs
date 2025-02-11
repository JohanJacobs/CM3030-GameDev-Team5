using System;
using UnityEngine;

public class NewAttributeValue
{
    public delegate void ValueDelegate(NewAttributeValue attributeValue, float oldValue, float newValue);

    public AttributeType Attribute { get; }

    public float BaseValue
    {
        get => _baseValue;
        set => SetBaseValue(value);
    }

    public float Value
    {
        get => _currentValue;
        set => SetCurrentValue(value);
    }

    public event ValueDelegate BaseValueChanged;
    public event ValueDelegate ValueChanged;

    private float _baseValue;
    private float _currentValue;

    public NewAttributeValue(AttributeType attribute)
    {
        Attribute = attribute;
    }

    public NewAttributeValue(AttributeType attribute, float defaultValue)
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
