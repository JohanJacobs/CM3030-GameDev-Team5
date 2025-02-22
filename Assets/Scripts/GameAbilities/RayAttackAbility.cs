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

        var ray = new Ray(origin, direction);
        var range = Range.Calculate(abilityInstance);

        if (Physics.Raycast(ray, out var hit, range, LayerMask))
        {
            var victim = hit.collider.GetComponentInParent<Creature>();
            if (victim)
            {
                var damageMin = DamageMin.Calculate(abilityInstance);
                var damageMax = DamageMax.Calculate(abilityInstance);

                attacker.DealDamage(victim, origin, Random.Range(damageMin, damageMax));
            }
        }

        // TEMP spawn tracer
        var pc = attacker.GetComponent<PlayerController>();
        if (pc)
        {
            pc.RandomSpawnBulletTracerFX(origin, direction, 1f);
        }
        // TEMP END

        abilityInstance.End();
    }
}