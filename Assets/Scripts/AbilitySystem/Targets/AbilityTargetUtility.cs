/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

Please view README file for detailed information

*/
using System.Collections.Generic;

public static class AbilityTargetUtility
{
    public delegate void EffectContextDelegate(EffectContext effectContext);

    private static readonly List<EffectHandle> TempTargetEffectHandles = new List<EffectHandle>(128);

    public static IEnumerable<EffectHandle> ApplyAbilityEffectsToTargets(AbilityInstance abilityInstance, Effect effect, IEnumerable<AbilityTarget> targets)
    {
        TempTargetEffectHandles.Clear();

        return ApplyAbilityEffectsToTargets(abilityInstance, new[] { effect }, targets);
    }

    public static IEnumerable<EffectHandle> ApplyAbilityEffectsToTargets(AbilityInstance abilityInstance, Effect[] effects, IEnumerable<AbilityTarget> targets)
    {
        TempTargetEffectHandles.Clear();

        foreach (var target in targets)
        {
            foreach (var effect in effects)
            {
                var effectHandle = ApplyAbilityEffectToTarget(abilityInstance, effect, target);
                if (effectHandle != null)
                {
                    TempTargetEffectHandles.Add(effectHandle);
                }
            }
        }

        return TempTargetEffectHandles;
    }

    public static EffectHandle ApplyAbilityEffectToTarget(AbilityInstance abilityInstance, Effect effect, in AbilityTarget target)
    {
        TempTargetEffectHandles.Clear();

        if (target.AbilitySystemComponent == null)
            return null;

        var source = abilityInstance.AbilitySystemComponent;

        var effectContext = source.CreateEffectContext(target.AbilitySystemComponent, abilityInstance);

        return source.ApplyEffectWithContext(effect, effectContext);
    }

    public static EffectHandle ApplyAbilityEffectToTarget(AbilityInstance abilityInstance, Effect effect, in AbilityTarget target, EffectContextDelegate effectContextDelegate)
    {
        TempTargetEffectHandles.Clear();

        if (target.AbilitySystemComponent == null)
            return null;

        var source = abilityInstance.AbilitySystemComponent;

        var effectContext = source.CreateEffectContext(target.AbilitySystemComponent, abilityInstance);

        effectContextDelegate?.Invoke(effectContext);

        return source.ApplyEffectWithContext(effect, effectContext);
    }
}