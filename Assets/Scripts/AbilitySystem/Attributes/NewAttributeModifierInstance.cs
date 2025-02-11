using UnityEngine;

public abstract class NewAttributeModifierInstance
{
    public NewAttributeModifier AttributeModifier { get; }
    public bool Post { get; }
    public int Index { get; } = ++_nextIndex;

    private static int _nextIndex = 0;

    protected NewAttributeModifierInstance(NewAttributeModifier attributeModifier)
    {
        AttributeModifier = attributeModifier;
        Post = AttributeModifier.Post;
    }

    protected NewAttributeModifierInstance(bool post)
    {
        AttributeModifier = null;
        Post = post;
    }

    public abstract void Apply(ref float value);
}

public class NewAttributeModifierInstanceWithModifier : NewAttributeModifierInstance
{
    private ScalarModifier _scalarModifier;

    public NewAttributeModifierInstanceWithModifier(NewAttributeModifier attributeModifier)
        : base(attributeModifier)
    {
        Debug.Assert(attributeModifier.Type != NewAttributeModifierType.Override);

        _scalarModifier = ScalarModifier.MakeFromAttributeModifier(attributeModifier);
    }

    public NewAttributeModifierInstanceWithModifier(ScalarModifier scalarModifier, bool post)
        : base(post)
    {
        _scalarModifier = scalarModifier;
    }

    public override void Apply(ref float value)
    {
        value = _scalarModifier.Calculate(value);
    }
}

public class NewAttributeModifierInstanceWithOverride : NewAttributeModifierInstance
{
    private float _scalarOverride;

    public NewAttributeModifierInstanceWithOverride(NewAttributeModifier attributeModifier)
        : base(attributeModifier)
    {
        Debug.Assert(attributeModifier.Type == NewAttributeModifierType.Override);

        _scalarOverride = attributeModifier.Value;
    }

    public NewAttributeModifierInstanceWithOverride(float scalarOverride, bool post)
        : base(post)
    {
        _scalarOverride = scalarOverride;
    }

    public override void Apply(ref float value)
    {
        value = _scalarOverride;
    }
}
