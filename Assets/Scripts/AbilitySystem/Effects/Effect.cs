/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

Effect.cs

*/

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public enum EffectDurationPolicy
{
    Instant,
    Duration,
    Infinite,
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

    public Magnitude Duration;
    public Magnitude Period;

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

    public bool HasTag(in Tag tag)
    {
        return Tags?.Contains(tag) ?? false;
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

    public virtual EffectInstance CreateEffectInstance(Effect effect, EffectContext context)
    {
        return new EffectInstance(effect, context);
    }

    protected virtual bool IsValid()
    {
        // check if effect logic is well-defined

        // TODO: formerly it was checking duration/period but these are context-dependent now

        return true;
    }
}
