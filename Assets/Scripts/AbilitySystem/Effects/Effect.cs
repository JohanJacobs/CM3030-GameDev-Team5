using System;
using UnityEngine;

public enum EffectDurationPolicy
{
    Instant,
    Duration,
    Infinite,
}

public enum EffectApplicationPolicy
{
    Instant,
    Periodic,
}

public enum EffectCancellationPolicy
{
    DoNothing,
    CancelAllModifiers,
}

[CreateAssetMenu(menuName = "Effects/Basic")]
public class Effect : ScriptableObject, IEffectLogic
{
    public AttributeModifier[] Modifiers;
    public EffectDurationPolicy DurationPolicy = EffectDurationPolicy.Instant;
    public EffectCancellationPolicy CancellationPolicy = EffectCancellationPolicy.CancelAllModifiers;
    public float Duration = 0f;
    public float Period = 0f;

    /// <summary>
    /// This effect's own tags, used to e.g. cancel all effects with tag.
    /// </summary>
    public Tag[] Tags;
    /// <summary>
    /// Tags that will block effect application if present in target AbilitySystem. Also blocks periodic effect applications.
    /// </summary>
    public Tag[] BlockTags;
    /// <summary>
    /// Tags that will end active effect if present in target AbilitySystem
    /// </summary>
    public Tag[] CancelTags;
    /// <summary>
    /// Tags that are added to target AbilitySystem as long as this effect is active.
    /// </summary>
    /// <remarks>
    /// Note that instant effects are not granting tags, but they're treated as being granted so i.e. can cancel effects/abilities.
    /// </remarks>
    public Tag[] GrantedTags;

    public bool HasDuration => Duration > 0f;
    public bool HasPeriod => Period > 0f;
    public bool HasBlockTags => BlockTags != null && BlockTags.Length > 0;
    public bool HasCancelTags => CancelTags != null && CancelTags.Length > 0;
    public bool HasGrantedTags => GrantedTags != null && GrantedTags.Length > 0;
    public bool Instant => DurationPolicy == EffectDurationPolicy.Instant;

    public bool Validate()
    {
        bool result = IsValid();

        Debug.Assert(result, "Effect definition is invalid");

        return result;
    }

    public virtual void ApplyEffect(EffectInstance effectInstance)
    {
    }

    public virtual void CancelEffect(EffectInstance effectInstance)
    {
    }

    public virtual void UpdateEffect(EffectInstance effectInstance, float deltaTime)
    {
    }

    protected virtual bool IsValid()
    {
        // check if effect logic is well-defined
        switch (DurationPolicy)
        {
            case EffectDurationPolicy.Instant:
                // TODO: do periodic instant effects make sense?
                return !HasPeriod;
            case EffectDurationPolicy.Duration:
                return HasDuration;
            case EffectDurationPolicy.Infinite:
                return true;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
