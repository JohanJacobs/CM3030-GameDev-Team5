using UnityEngine;

public abstract class AttributeModifierInstance
{
    public AttributeModifier AttributeModifier { get; }
    public bool Post { get; }
    public int Index { get; } = ++_nextIndex;

    private static int _nextIndex = 0;

    protected AttributeModifierInstance(AttributeModifier attributeModifier)
    {
        AttributeModifier = attributeModifier;
        Post = AttributeModifier.Post;
    }

    protected AttributeModifierInstance(bool post)
    {
        AttributeModifier = null;
        Post = post;
    }

    public abstract void Apply(ref float value);
}

public class AttributeModifierInstanceWithModifier : AttributeModifierInstance
{
    private ScalarModifier _scalarModifier;

    public AttributeModifierInstanceWithModifier(AttributeModifier attributeModifier)
        : base(attributeModifier)
    {
        Debug.Assert(attributeModifier.Type != NewAttributeModifierType.Override);

        _scalarModifier = ScalarModifier.MakeFromAttributeModifier(attributeModifier);
    }

    public AttributeModifierInstanceWithModifier(ScalarModifier scalarModifier, bool post)
        : base(post)
    {
        _scalarModifier = scalarModifier;
    }

    public override void Apply(ref float value)
    {
        value = _scalarModifier.Calculate(value);
    }
}

public class AttributeModifierInstanceWithOverride : AttributeModifierInstance
{
    private float _scalarOverride;

    public AttributeModifierInstanceWithOverride(AttributeModifier attributeModifier)
        : base(attributeModifier)
    {
        Debug.Assert(attributeModifier.Type == NewAttributeModifierType.Override);

        _scalarOverride = attributeModifier.Value;
    }

    public AttributeModifierInstanceWithOverride(float scalarOverride, bool post)
        : base(post)
    {
        _scalarOverride = scalarOverride;
    }

    public override void Apply(ref float value)
    {
        value = _scalarOverride;
    }
}
