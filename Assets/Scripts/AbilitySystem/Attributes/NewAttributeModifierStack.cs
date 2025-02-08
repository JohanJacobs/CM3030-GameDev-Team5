using System;
using System.Collections.Generic;

public class NewAttributeModifierStack
{
    public NewAbilitySystemComponent Owner => _weakOwner.TryGetTarget(out var owner) ? owner : null;

    private readonly WeakReference<NewAbilitySystemComponent> _weakOwner;
    private readonly List<NewAttributeModifierInstance> _modifiers = new List<NewAttributeModifierInstance>();

    private ScalarModifier _permanentModifier = ScalarModifier.MakeIdentity();
    private ScalarModifier _permanentPostModifier = ScalarModifier.MakeIdentity();

    public NewAttributeModifierStack(NewAbilitySystemComponent owner)
    {
        _weakOwner = new WeakReference<NewAbilitySystemComponent>(owner);
    }

    public NewAttributeModifierInstance AddModifier(NewAttributeModifier modifier)
    {
        if (modifier.Permanent)
        {
            var scalarModifier = ScalarModifier.MakeFromAttributeModifier(modifier);

            if (modifier.Post)
                _permanentPostModifier.Combine(scalarModifier);
            else
                _permanentModifier.Combine(scalarModifier);

            return null;
        }

        var instance = new NewAttributeModifierInstance(modifier);

        _modifiers.Add(instance);

        return instance;
    }

    public NewAttributeModifierInstance AddModifier(ScalarModifier scalarModifier, bool permanent, bool post)
    {
        if (permanent)
        {
            if (post)
                _permanentPostModifier.Combine(scalarModifier);
            else
                _permanentModifier.Combine(scalarModifier);

            return null;
        }

        var instance = new NewAttributeModifierInstance(scalarModifier, false, post);

        _modifiers.Add(instance);

        return instance;
    }

    public void RemoveModifier(NewAttributeModifierInstance instance)
    {
        var removed = _modifiers.Remove(instance);
        if (!removed)
            return;

    }
}
