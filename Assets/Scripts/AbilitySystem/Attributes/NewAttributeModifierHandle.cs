using System;

public sealed class NewAttributeModifierHandle
{
    public NewAbilitySystemComponent AbilitySystemComponent => _weakAbilitySystemComponent.TryGetTarget(out var asc) ? asc : null;
    public NewAttributeModifierStack ModifierStack => _weakModifierStack.TryGetTarget(out var stack) ? stack : null;
    public NewAttributeModifierInstance ModifierInstance => _weakModifierInstance.TryGetTarget(out var instance) ? instance : null;

    private readonly WeakReference<NewAbilitySystemComponent> _weakAbilitySystemComponent;
    private readonly WeakReference<NewAttributeModifierStack> _weakModifierStack;
    private readonly WeakReference<NewAttributeModifierInstance> _weakModifierInstance;

    public NewAttributeModifierHandle(NewAbilitySystemComponent asc, NewAttributeModifierStack modifierStack, NewAttributeModifierInstance modifierInstance)
    {
        _weakAbilitySystemComponent = new WeakReference<NewAbilitySystemComponent>(asc);
        _weakModifierStack = new WeakReference<NewAttributeModifierStack>(modifierStack);
        _weakModifierInstance = new WeakReference<NewAttributeModifierInstance>(modifierInstance);
    }
}
