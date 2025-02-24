using System;

public sealed class EffectContext
{
    public AbilitySystemComponent Source => _weakSource.TryGetTarget(out var asc) ? asc : null;
    public AbilitySystemComponent Target => _weakTarget.TryGetTarget(out var asc) ? asc : null;

    public AbilityInstance AbilityInstance
    {
        get
        {
            if (_weakAbilityInstance == null)
                return null;
            return _weakAbilityInstance.TryGetTarget(out var abilityInstance) ? abilityInstance : null;
        }
    }

    private readonly WeakReference<AbilitySystemComponent> _weakSource;
    private readonly WeakReference<AbilitySystemComponent> _weakTarget;
    private readonly WeakReference<AbilityInstance> _weakAbilityInstance;

    public EffectContext(AbilitySystemComponent source, AbilitySystemComponent target)
    {
        _weakSource = new WeakReference<AbilitySystemComponent>(source);
        _weakTarget = new WeakReference<AbilitySystemComponent>(target);
    }

    public EffectContext(AbilitySystemComponent source, AbilitySystemComponent target, AbilityInstance abilityInstance)
        : this(source, target)
    {
        _weakAbilityInstance = new WeakReference<AbilityInstance>(abilityInstance);
    }
}
