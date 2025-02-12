using System;

public sealed class AttributeModifierHandle
{
    public AbilitySystemComponent AbilitySystemComponent => _weakAbilitySystemComponent.TryGetTarget(out var asc) ? asc : null;
    public AttributeModifierStack ModifierStack => _weakModifierStack.TryGetTarget(out var stack) ? stack : null;
    public AttributeModifierInstance ModifierInstance => _weakModifierInstance.TryGetTarget(out var instance) ? instance : null;

    private readonly WeakReference<AbilitySystemComponent> _weakAbilitySystemComponent;
    private readonly WeakReference<AttributeModifierStack> _weakModifierStack;
    private readonly WeakReference<AttributeModifierInstance> _weakModifierInstance;

    public AttributeModifierHandle(AbilitySystemComponent asc, AttributeModifierStack modifierStack, AttributeModifierInstance modifierInstance)
    {
        _weakAbilitySystemComponent = new WeakReference<AbilitySystemComponent>(asc);
        _weakModifierStack = new WeakReference<AttributeModifierStack>(modifierStack);
        _weakModifierInstance = new WeakReference<AttributeModifierInstance>(modifierInstance);
    }

    public void Clear()
    {
        _weakModifierStack.SetTarget(null);
        _weakModifierInstance.SetTarget(null);
    }
}
