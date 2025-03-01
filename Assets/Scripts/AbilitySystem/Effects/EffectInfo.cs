public struct EffectAttributeModifierInfo
{
    public AttributeType Attribute;
    public float Value;
}

public struct EffectInfo
{
    public Effect Effect;
    public EffectAttributeModifierInfo[] AttributeModifiers;
    public float TimeLeft;
    public float TimeLeftFraction;
    public int Stacks;
}