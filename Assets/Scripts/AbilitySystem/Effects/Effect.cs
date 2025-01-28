using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class EffectHandle
{
    private readonly WeakReference<AbilitySystemComponent> _weakAbilitySystemComponent;
    private readonly WeakReference<Effect> _weakEffect;
    private readonly WeakReference<object> _weakInternalEffect;
}

[CreateAssetMenu]
public sealed class Effect : ScriptableObject
{
    public List<AttributeModifier> Modifiers;


}
