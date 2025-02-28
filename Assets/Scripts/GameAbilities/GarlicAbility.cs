/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

GarlicAbility.cs

*/

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

        var targetQuery = new AbilityTargetQuery()
        {
            LayerMask = LayerMask,
            Origin = asc.transform.position,
            Range = range,
        };

        var targets = AbilityTargetSelector.GetAreaTargets(targetQuery);

        AbilityTargetUtility.ApplyAbilityEffectsToTargets(abilityInstance, TargetEffects, targets);
    }

    public override void ActivateAbility(AbilityInstance abilityInstance)
    {
        var data = abilityInstance.GetData<GarlicAbilityInstanceData>();

        data.TimeToNextTrigger = 0;
    }
}
