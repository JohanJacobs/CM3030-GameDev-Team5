using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ray Attack")]
public class RayAttackAbility : AttackAbility
{
    public LayerMask LayerMask;

    public override void ActivateAbility(AbilityInstance abilityInstance)
    {
        var asc = abilityInstance.AbilitySystemComponent;
        var attacker = asc.GetComponent<Creature>();

        GetDefaultAttackOriginAndDirection(abilityInstance, out var origin, out var direction);

        // HACK: ray comes from attacker center with slight vertical offset
        var ray = new Ray(attacker.transform.position + Vector3.up * 0.5f, direction);

        var range = Range.Calculate(abilityInstance);
        var damageMin = DamageMin.Calculate(abilityInstance);
        var damageMax = DamageMax.Calculate(abilityInstance);

        var damage = Random.Range(damageMin, damageMax);

        if (Physics.Raycast(ray, out var hit, range, LayerMask))
        {
            // HACK: update direction for visuals
            // {
            //     direction = hit.point - origin;
            //
            //     direction.y = 0;
            //     direction.Normalize();
            // }

            var victim = hit.collider.GetComponentInParent<Creature>();
            if (victim)
            {
                var effectContext = asc.CreateEffectContext(victim.AbilitySystemComponent, abilityInstance);

                effectContext.SetValue(DamageEffect.AmountSetByCaller, damage);

                asc.ApplyEffectWithContext(DamageEffect, effectContext);
            }
        }

        NotifyAttackCommitted(abilityInstance, origin, direction, damage);

        abilityInstance.End();
    }
}