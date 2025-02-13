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

public abstract class Ability : ScriptableObject, IAbilityLogic
{
    public bool ActivateOnGranted = false;

    public Effect CostEffect;
    public Effect CooldownEffect;

    public Tag[] Tags;
    public Tag[] BlockTags;
    public Tag[] CancelTags;

    public AbilityInstanceDataClass AbilityInstanceDataClass;

    public virtual void HandleAbilityAdded(AbilityInstance abilityInstance)
    {
    }

    public virtual void HandleAbilityRemoved(AbilityInstance abilityInstance)
    {
    }

    public virtual void ActivateAbility(AbilityInstance abilityInstance)
    {
    }

    public virtual bool CanActivateAbility(AbilityInstance abilityInstance)
    {
        return true;
    }

    public virtual bool CommitAbility(AbilityInstance abilityInstance)
    {
        return CanActivateAbility(abilityInstance);
    }

    public virtual void EndAbility(AbilityInstance abilityInstance)
    {
    }

    public virtual void UpdateAbility(AbilityInstance abilityInstance, float deltaTime)
    {
    }
}
