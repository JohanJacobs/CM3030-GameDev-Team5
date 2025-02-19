using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectInstance
{
    public Effect Effect { get; }
    public EffectContext Context { get; }
    public bool Expired => _expired;
    public bool Canceled => _canceled;
    public bool Active => !Expired && !Canceled;
    public bool Inactive => !Active;
    public float TimeRemaining => _timeLeftToExpiration;

    public float TimeRemainingFraction
    {
        get
        {
            if (_hasDuration)
                return Mathf.Clamp(_timeLeftToExpiration / Effect.Duration, 0f, 1f);

            return 1f;
        }
    }

    private readonly List<AttributeModifierHandle> _modifiers = new List<AttributeModifierHandle>();

    private readonly bool _hasDuration;
    private readonly bool _hasPeriod;

    private IEffectLogic _effectLogic;

    private float _timeLeftToExpiration;
    private float _timeLeftToPeriodicApplication;

    private bool _expired;
    private bool _canceled;

    public EffectInstance(Effect effect, EffectContext context)
    {
        Effect = effect;
        Context = context;

        _effectLogic = effect;

        switch (effect.DurationPolicy)
        {
            case EffectDurationPolicy.Instant:
                _hasDuration = false;
                _hasPeriod = false;
                break;
            case EffectDurationPolicy.Duration:
                _hasDuration = effect.HasDuration;
                _hasPeriod = effect.HasPeriod;
                break;
            case EffectDurationPolicy.Infinite:
                _hasDuration = false;
                _hasPeriod = effect.HasPeriod;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_hasDuration)
        {
            _timeLeftToExpiration = effect.Duration;
        }

        if (_hasPeriod)
        {
            _timeLeftToPeriodicApplication = effect.Period;
        }
    }

    public void Destroy()
    {
        Debug.Assert(!Active, "Destroying active effect instance probably points to logic error");

        // NOTE: might be required to break cross-reference chain
        _effectLogic = null;
    }

    public void Apply()
    {
        Debug.Assert(Active);

        foreach (var modifier in Effect.Modifiers)
        {
            var handle = Context.Target.ApplyAttributeModifier(modifier);
            if (handle == null)
                continue;

            _modifiers.Add(handle);
        }

        _effectLogic.ApplyEffect(this);
    }

    public void ApplyIfPossible()
    {
        if (Context.Target.CanApplyEffect(Effect))
        {
            Apply();
        }
    }

    public void Cancel()
    {
        if (!Active)
            return;

        _canceled = true;

        RevokeEffect();
    }

    public void Update(float deltaTime)
    {
        if (!Active)
            return;

        if (_hasPeriod)
        {
            _timeLeftToPeriodicApplication -= deltaTime;

            if (!(_timeLeftToPeriodicApplication > 0f))
            {
                HandlePeriodicApplication();
            }
        }

        _effectLogic.UpdateEffect(this, deltaTime);

        if (_hasDuration)
        {
            _timeLeftToExpiration -= deltaTime;

            if (!(_timeLeftToExpiration > 0f))
            {
                HandleExpiration();
            }
        }
    }

    private void CancelAllModifiers()
    {
        foreach (var handle in _modifiers)
        {
            Context.Target.CancelAttributeModifier(handle);
        }

        _modifiers.Clear();
    }

    private void HandleExpiration()
    {
        _expired = true;

        RevokeEffect();
    }

    private void HandlePeriodicApplication()
    {
        _timeLeftToPeriodicApplication += Effect.Period;

        ApplyIfPossible();
    }

    private void RevokeEffect()
    {
        _effectLogic.CancelEffect(this);

        switch (Effect.CancellationPolicy)
        {
            case EffectCancellationPolicy.DoNothing:
                break;
            case EffectCancellationPolicy.CancelAllModifiers:
                CancelAllModifiers();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
