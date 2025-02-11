using System;

public class EffectContext
{
    public NewAbilitySystemComponent Source => _weakSource.TryGetTarget(out var asc) ? asc : null;
    public NewAbilitySystemComponent Target => _weakTarget.TryGetTarget(out var asc) ? asc : null;
    public AbilityInstance Ability { get; }

    private readonly WeakReference<NewAbilitySystemComponent> _weakSource;
    private readonly WeakReference<NewAbilitySystemComponent> _weakTarget;

    public EffectContext(NewAbilitySystemComponent source, NewAbilitySystemComponent target)
    {
        _weakSource = new WeakReference<NewAbilitySystemComponent>(source);
        _weakTarget = new WeakReference<NewAbilitySystemComponent>(target);
    }

    public EffectContext(NewAbilitySystemComponent source, NewAbilitySystemComponent target, AbilityInstance ability)
        : this(source, target)
    {
        Ability = ability;
    }
}
