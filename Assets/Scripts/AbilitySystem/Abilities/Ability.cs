using UnityEngine;

public enum AbilityInputPolicy
{
    None,
    TryActivateOnInputPressed,
    TryActivateWhileInputPressed,
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

    public AbilityInstanceDataClass AbilityInstanceDataClass = new AbilityInstanceDataClass(typeof(DefaultAbilityInstanceData));

    public AbilityInputPolicy InputPolicy = AbilityInputPolicy.None;
    public Tag InputTag;

    public InputMappingContext InputMappingContext;

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

    public virtual void HandleAbilityInputActionPressed(AbilityInstance abilityInstance, InputAction action)
    {
    }

    public virtual void HandleAbilityInputActionReleased(AbilityInstance abilityInstance, InputAction action)
    {
    }
}
