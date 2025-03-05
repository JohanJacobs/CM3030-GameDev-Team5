/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

RayAttackAbility.cs

*/

using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ray Attack")]
public class RayAttackAbility : AttackAbility
{
    public LayerMask LayerMask;

    public override void ActivateAbility(AbilityInstance abilityInstance)
    {
        var asc = abilityInstance.AbilitySystemComponent;
        var attacker = asc.GetComponent<Creature>();

        GetOwnerAim(abilityInstance, out var origin, out var direction);

        var ray = new Ray(origin, direction);

        var range = Range.Calculate(abilityInstance);
        var damageMin = DamageMin.Calculate(abilityInstance);
        var damageMax = DamageMax.Calculate(abilityInstance);

        var damage = Random.Range(damageMin, damageMax);

        var targetQuery = new AbilityTargetQuery()
        {
            Direction = direction,
            LayerMask = LayerMask,
            Origin = origin,
            Range = range,
        };

        var targets = AbilityTargetSelector.GetRaycastTargetsSingle(targetQuery).ToArray();

        foreach (var target in targets)
        {
            AbilityTargetUtility.ApplyAbilityEffectToTarget(abilityInstance, DamageEffect, target, effectContext =>
            {
                effectContext.SetValue(DamageEffect.AmountSetByCaller, damage);
            });
        }

        AbilityTargetUtility.ApplyAbilityEffectsToTargets(abilityInstance, TargetEffects, targets);

        NotifyAttackCommitted(abilityInstance, origin, direction);

        abilityInstance.End();
    }
}
