/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

Please view README file for detailed information

*/
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