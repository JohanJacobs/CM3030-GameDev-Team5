using System;
using JetBrains.Annotations;
using UnityEngine;

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

    /// <summary>
    /// Ability's own tags
    /// </summary>
    public Tag[] Tags;
    /// <summary>
    /// Tags that will block ability activation if present
    /// </summary>
    public Tag[] BlockTags;
    /// <summary>
    /// Tags that will end active ability
    /// </summary>
    public Tag[] CancelTags;
    /// <summary>
    /// Tags that are added to AbilitySystem as long as it has this ability, either active or not
    /// </summary>
    public Tag[] GrantedTags;

    public AbilityInstanceDataClass AbilityInstanceDataClass;

    public bool HasBlockTags => BlockTags != null && BlockTags.Length > 0;
    public bool HasCancelTags => CancelTags != null && CancelTags.Length > 0;
    public bool HasGrantedTags => GrantedTags != null && GrantedTags.Length > 0;

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
