using System;

public sealed class EffectHandle
{
    public AbilitySystemComponent AbilitySystemComponent => _weakAbilitySystemComponent.TryGetTarget(out var asc) ? asc : null;
    public EffectInstance EffectInstance => _weakEffectInstance.TryGetTarget(out var instance) ? instance : null;

    private readonly WeakReference<AbilitySystemComponent> _weakAbilitySystemComponent;
    private readonly WeakReference<EffectInstance> _weakEffectInstance;

    public EffectHandle(AbilitySystemComponent asc, EffectInstance effectInstance)
    {
        _weakAbilitySystemComponent = new WeakReference<AbilitySystemComponent>(asc);
        _weakEffectInstance = new WeakReference<EffectInstance>(effectInstance);
    }

    public void Clear()
    {
        _weakEffectInstance.SetTarget(null);
    }
}
