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

public sealed class EffectHandle
{
    public AbilitySystemComponent AbilitySystemComponent => _weakAbilitySystemComponent.TryGetTarget(out var asc) ? asc : null;
    public Effect Effect => _weakEffect.TryGetTarget(out var effect) ? effect : null;
    public object InternalEffect => _weakInternalEffect.TryGetTarget(out var internalEffect) ? internalEffect : null;

    private readonly WeakReference<AbilitySystemComponent> _weakAbilitySystemComponent;
    private readonly WeakReference<Effect> _weakEffect;
    private readonly WeakReference<object> _weakInternalEffect;

    public EffectHandle(AbilitySystemComponent asc, Effect effect, object internalEffect)
    {
        _weakAbilitySystemComponent = new WeakReference<AbilitySystemComponent>(asc);
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
    public List<AttributeModifier> Modifiers;
    public EffectDurationPolicy DurationPolicy = EffectDurationPolicy.Instant;
    public EffectApplicationPolicy ApplicationPolicy = EffectApplicationPolicy.Instant;
    public EffectCancellationPolicy CancellationPolicy = EffectCancellationPolicy.CancelAllModifiers;
    public float Duration = 1f;
    public float Period = 0.1f;
}
