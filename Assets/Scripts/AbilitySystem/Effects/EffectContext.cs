/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

EffectContext.cs

*/

using System;
using System.Collections.Generic;

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

    private readonly Dictionary<Tag, object> _values = new Dictionary<Tag, object>();

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

    public void SetValue<T>(in Tag tag, T value)
    {
        _values[tag] = value;
    }

    public bool GetValue<T>(in Tag tag, out T value)
    {
        if (_values.TryGetValue(tag, out var obj))
        {
            if (obj is T concreteValue)
            {
                value = concreteValue;
                return true;
            }
        }

        value = default;
        return false;
    }

    public bool HasValue(in Tag tag)
    {
        return _values.ContainsKey(tag);
    }
}
