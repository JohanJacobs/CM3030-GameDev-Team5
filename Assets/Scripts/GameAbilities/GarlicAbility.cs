using System.Collections.Generic;
using UnityEngine;

[AbilityInstanceDataClass]
public class GarlicAbilityInstanceData : AbilityInstanceData
{
    public float TimeToNextTrigger;
}

[CreateAssetMenu(menuName = "Abilities/Garlic")]
public class GarlicAbility : Ability
{
    public LayerMask LayerMask;

    public Magnitude Range = new Magnitude()
    {
        Calculation = MagnitudeCalculation.AttributeAdd,
        Attribute = AttributeType.AttackRange,
        Value = 4f,
    };

    public Magnitude Rate = new Magnitude()
    {
        Calculation = MagnitudeCalculation.Simple,
        Value = 0.1f,
    };

    public Effect[] TargetEffects;

    public GarlicAbility()
    {
        ActivateOnGranted = true;
        AbilityInstanceDataClass = new AbilityInstanceDataClass(typeof(GarlicAbilityInstanceData));
    }

    public override void UpdateAbility(AbilityInstance abilityInstance, float deltaTime)
    {
        var data = abilityInstance.GetData<GarlicAbilityInstanceData>();

        data.TimeToNextTrigger -= deltaTime;

        if (data.TimeToNextTrigger > 0)
            return;

        var range = abilityInstance.CalculateMagnitude(Range);
        var rate = abilityInstance.CalculateMagnitude(Rate);

        Debug.Assert(range > 0);
        Debug.Assert(rate > 0);

        data.TimeToNextTrigger += 1f / rate;

        var asc = abilityInstance.AbilitySystemComponent;

        ApplyEffectsToTargetsInRange(abilityInstance, asc, asc.transform.position, range, LayerMask, TargetEffects);
    }

    public override void ActivateAbility(AbilityInstance abilityInstance)
    {
        var data = abilityInstance.GetData<GarlicAbilityInstanceData>();

        data.TimeToNextTrigger = 0;
    }

    private static void ApplyEffectsToTargetsInRange(AbilityInstance abilityInstance, AbilitySystemComponent source, Vector3 origin, float range, LayerMask layerMask, Effect[] effects)
    {
        // TODO: consider OverlapSphereNonAlloc
        var colliders = Physics.OverlapSphere(origin, range, layerMask);

        foreach (var collider in colliders)
        {
            var monster = collider.GetComponentInParent<Monster>();
            if (monster == null)
                continue;

            foreach (var effect in effects)
            {
                source.ApplyEffectToTarget(effect, monster.AbilitySystemComponent, abilityInstance);
            }
        }
    }
}