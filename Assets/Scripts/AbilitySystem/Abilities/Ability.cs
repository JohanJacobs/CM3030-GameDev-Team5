using UnityEngine;
using System;
using JetBrains.Annotations;

[Serializable]
public sealed class AbilityCostDefinition
{
    public AttributeType Attribute;
    public float Value;
}

[Serializable]
public sealed class AbilityCooldownDefinition
{
    public float Value;
}

[CreateAssetMenu]
public class Ability : ScriptableObject
{
    public bool ActivateOnGranted = false;

    public Effect CostEffect;
    public Effect CooldownEffect;

    public Tag[] Tags;
    public Tag[] BlockTags;
    public Tag[] CancelTags;

    public AbilityLogicClass AbilityLogicClass;
}
