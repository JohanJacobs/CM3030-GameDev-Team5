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
        var damageMin = DamageMin.Calculate(abilityInstance);
        var damageMax = DamageMax.Calculate(abilityInstance);

        var damage = Random.Range(damageMin, damageMax);

        if (Physics.Raycast(ray, out var hit, range, LayerMask))
        {
            var victim = hit.collider.GetComponentInParent<Creature>();
            if (victim)
            {
                attacker.DealDamage(victim, origin, damage);
            }
        }

        NotifyAttackCommitted(abilityInstance, origin, direction, damage);

        abilityInstance.End();
    }
}