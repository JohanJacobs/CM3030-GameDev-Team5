using System;

public sealed class NewAttributeModifierHandle
{
    private readonly WeakReference<NewAbilitySystemComponent> _weakAbilitySystemComponent;
    private readonly WeakReference<NewAttributeModifierInstance> _weakModifierInstance;

    public NewAttributeModifierHandle(NewAbilitySystemComponent asc, NewAttributeModifierInstance modifierInstance)
    {
        _weakAbilitySystemComponent = new WeakReference<NewAbilitySystemComponent>(asc);
        _weakModifierInstance = new WeakReference<NewAttributeModifierInstance>(modifierInstance);
    }
}
