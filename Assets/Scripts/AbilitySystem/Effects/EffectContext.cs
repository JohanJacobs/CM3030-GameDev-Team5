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

public class EffectContext
{
    public AbilitySystemComponent Source => _weakSource.TryGetTarget(out var asc) ? asc : null;
    public AbilitySystemComponent Target => _weakTarget.TryGetTarget(out var asc) ? asc : null;
    public AbilityInstance Ability { get; }

    private readonly WeakReference<AbilitySystemComponent> _weakSource;
    private readonly WeakReference<AbilitySystemComponent> _weakTarget;

    public EffectContext(AbilitySystemComponent source, AbilitySystemComponent target)
    {
        _weakSource = new WeakReference<AbilitySystemComponent>(source);
        _weakTarget = new WeakReference<AbilitySystemComponent>(target);
    }

    public EffectContext(AbilitySystemComponent source, AbilitySystemComponent target, AbilityInstance ability)
        : this(source, target)
    {
        Ability = ability;
    }
}
