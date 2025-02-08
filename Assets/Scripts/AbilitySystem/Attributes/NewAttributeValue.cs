using System;

public class NewAttributeValue
{
    public NewAttributeSetInstance Owner => _weakOwner.TryGetTarget(out var owner) ? owner : null;

    public AttributeType Attribute { get; }

    public float BaseValue
    {
        get => _baseValue;
        set => _baseValue = value;
    }

    public float Value
    {
        get => _currentValue;
        set => _currentValue = value;
    }

    private readonly WeakReference<NewAttributeSetInstance> _weakOwner;

    private float _baseValue;
    private float _currentValue;

    public NewAttributeValue(NewAttributeSetInstance owner, AttributeType attribute)
    {
        if (owner is null)
            throw new ArgumentNullException(nameof(owner));

        Attribute = attribute;

        _weakOwner = new WeakReference<NewAttributeSetInstance>(owner);
    }

    public NewAttributeValue(NewAttributeSetInstance owner, AttributeType attribute, float defaultValue)
        : this(owner, attribute)
    {
        _baseValue = defaultValue;
        _currentValue = defaultValue;
    }
}
