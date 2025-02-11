using System;

public sealed class NewEffectHandle
{
    public NewAbilitySystemComponent AbilitySystemComponent => _weakAbilitySystemComponent.TryGetTarget(out var asc) ? asc : null;
    public EffectInstance EffectInstance => _weakEffectInstance.TryGetTarget(out var instance) ? instance : null;

    private readonly WeakReference<NewAbilitySystemComponent> _weakAbilitySystemComponent;
    private readonly WeakReference<EffectInstance> _weakEffectInstance;

    public NewEffectHandle(NewAbilitySystemComponent asc, EffectInstance effectInstance)
    {
        _weakAbilitySystemComponent = new WeakReference<NewAbilitySystemComponent>(asc);
        _weakEffectInstance = new WeakReference<EffectInstance>(effectInstance);
    }
}
