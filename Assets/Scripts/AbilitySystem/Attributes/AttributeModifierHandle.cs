/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

AttributeModifierHandle.cs

*/

using System;

public sealed class AttributeModifierHandle : GenericInstanceHandle<AttributeModifierInstance>
{
    public AttributeModifierStack ModifierStack => _weakModifierStack.TryGetTarget(out var stack) ? stack : null;
    public AttributeModifierInstance ModifierInstance => Instance;

    private readonly WeakReference<AttributeModifierStack> _weakModifierStack;

    public AttributeModifierHandle(AbilitySystemComponent asc, AttributeModifierStack modifierStack, AttributeModifierInstance modifierInstance)
        : base(asc, modifierInstance)
    {
        _weakModifierStack = new WeakReference<AttributeModifierStack>(modifierStack);
    }

    public override void Clear()
    {
        base.Clear();

        _weakModifierStack.SetTarget(null);
    }
}
