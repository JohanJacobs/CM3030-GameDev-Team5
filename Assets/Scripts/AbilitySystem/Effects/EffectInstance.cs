using System;
using System.Collections.Generic;

public class EffectInstance
{
    public Effect Effect { get; }
    public EffectContext Context { get; }
    public bool Expired => _expired;

    private readonly List<NewAttributeModifierHandle> _modifiers = new List<NewAttributeModifierHandle>();

    private readonly bool _hasDuration;
    private readonly bool _hasPeriod;

    private float _timeLeftToExpiration;
    private float _timeLeftToPeriodicApplication;

    private bool _expired;

    public EffectInstance(Effect effect, EffectContext context)
    {
        Effect = effect;
        Context = context;

        switch (effect.DurationPolicy)
        {
            case EffectDurationPolicy.Instant:
                _hasDuration = false;
                _hasPeriod = false;
                _expired = true;
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

    public void Apply()
    {
        foreach (var modifier in Effect.Modifiers)
        {
            var handle = Context.Target.ApplyAttributeModifier(modifier);
            if (handle == null)
                continue;

            _modifiers.Add(handle);
        }
    }

    public void Cancel()
    {
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

    public void Update(float deltaTime)
    {
        if (_expired)
            return;

        if (_hasDuration)
        {
            _timeLeftToExpiration -= deltaTime;

            if (!(_timeLeftToExpiration > 0f))
            {
                HandleExpiration();
            }
        }

        // NOTE: effect that just expired is still allowed to trigger periodic application one last time
        if (_hasPeriod)
        {
            _timeLeftToPeriodicApplication -= deltaTime;

            if (!(_timeLeftToPeriodicApplication > 0f))
            {
                HandlePeriodicApplication();
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
    }

    private void HandlePeriodicApplication()
    {
        _timeLeftToPeriodicApplication += Effect.Period;

        Apply();
    }
}
