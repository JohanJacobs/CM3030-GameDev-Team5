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
    public bool SelfTargeted => Context.Target == Context.Source;
    public float TimeRemaining => _timeLeftToExpiration;

    public float TimeRemainingFraction
    {
        get
        {
            if (_duration > 0)
                return Mathf.Clamp(_timeLeftToExpiration / _duration, 0f, 1f);

            return 1f;
        }
    }

    private readonly List<AttributeModifierHandle> _modifiers = new List<AttributeModifierHandle>();

    private IEffectLogic _effectLogic;

    private float _duration;
    private float _period;

    private float _timeLeftToExpiration;
    private float _timeLeftToPeriodicApplication;

    private bool _expired;
    private bool _canceled;

    public EffectInstance(Effect effect, EffectContext context)
    {
        Effect = effect;
        Context = context;

        _effectLogic = effect;

        bool wantsDuration;
        bool wantsPeriod;

        switch (effect.DurationPolicy)
        {
            case EffectDurationPolicy.Instant:
                wantsDuration = false;
                wantsPeriod = false;
                break;
            case EffectDurationPolicy.Duration:
                wantsDuration = true;
                wantsPeriod = true;
                break;
            case EffectDurationPolicy.Infinite:
                wantsDuration = false;
                wantsPeriod = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (wantsDuration)
        {
            UpdateDuration(false);
        }

        if (wantsPeriod)
        {
            UpdatePeriod(false);
        }
    }

    public virtual void Destroy()
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

        if (_period > 0)
        {
            _timeLeftToPeriodicApplication -= deltaTime;

            if (!(_timeLeftToPeriodicApplication > 0f))
            {
                HandlePeriodicApplication();
            }
        }

        _effectLogic.UpdateEffect(this, deltaTime);

        if (_duration > 0)
        {
            // TODO: update duration magnitude?

            _timeLeftToExpiration -= deltaTime;

            if (!(_timeLeftToExpiration > 0f))
            {
                HandleExpiration();
            }
        }
    }

    public T GetEffect<T>() where T : Effect
    {
        if (Effect is T concreteEffect)
            return concreteEffect;

        throw new InvalidOperationException($"Requested invalid effect type {typeof(T)}");
    }

    public float CalculateMagnitude(Magnitude magnitude)
    {
        return magnitude.Calculate(Context);
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
        UpdatePeriod();

        _timeLeftToPeriodicApplication += _period;

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

    private void UpdateDuration(bool throwIfInvalid = true)
    {
        var duration = Effect.Duration.Calculate(Context);

        if (duration > 0)
        {
            // NOTE: some small duration is required to let the system do its job
            _duration = Mathf.Max(duration, 0.05f);
            _timeLeftToExpiration = _duration;
        }
        else
        {
            _duration = 0;

            if (throwIfInvalid)
                throw new Exception("Evaluated effect duration is 0");
        }
    }

    private void UpdatePeriod(bool throwIfInvalid = true)
    {
        var period = Effect.Period.Calculate(Context);
        if (period > 0)
        {
            _period = period;
            _timeLeftToPeriodicApplication = _period;
        }
        else
        {
            _period = 0;

            if (throwIfInvalid)
                throw new Exception("Evaluated effect period is 0");
        }
    }
}
