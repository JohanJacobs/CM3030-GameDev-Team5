using UnityEngine;
using System;
using System.Collections.Generic;

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

public sealed class OldEffectHandle
{
    public OldAbilitySystemComponent AbilitySystemComponent => _weakAbilitySystemComponent.TryGetTarget(out var asc) ? asc : null;
    public Effect Effect => _weakEffect.TryGetTarget(out var effect) ? effect : null;
    public object InternalEffect => _weakInternalEffect.TryGetTarget(out var internalEffect) ? internalEffect : null;

    private readonly WeakReference<OldAbilitySystemComponent> _weakAbilitySystemComponent;
    private readonly WeakReference<Effect> _weakEffect;
    private readonly WeakReference<object> _weakInternalEffect;

    public OldEffectHandle(OldAbilitySystemComponent asc, Effect effect, object internalEffect)
    {
        _weakAbilitySystemComponent = new WeakReference<OldAbilitySystemComponent>(asc);
        _weakEffect = new WeakReference<Effect>(effect);
        _weakInternalEffect = new WeakReference<object>(internalEffect);
    }

    public bool CancelEffect()
    {
        var asc = AbilitySystemComponent;
        if (asc == null)
            return false;

        asc.CancelEffect(this);

        return true;
    }
}

[CreateAssetMenu]
public sealed class Effect : ScriptableObject
{
    public NewAttributeModifier[] Modifiers;
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
    /// Tags that are added to target AbilitySystem as long as this effect is active.
    /// </summary>
    /// <remarks>
    /// Note that instant effects are not granting tags.
    /// </remarks>
    public Tag[] GrantedTags;

    public bool HasDuration => Duration > 0f;
    public bool HasPeriod => Period > 0f;
    public bool IsInstant => DurationPolicy == EffectDurationPolicy.Instant;
    public bool IsFinite => DurationPolicy == EffectDurationPolicy.Duration && HasDuration;
    public bool IsInfinite => DurationPolicy == EffectDurationPolicy.Infinite;
}
