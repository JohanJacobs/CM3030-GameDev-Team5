using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Projectile Attack")]
public class ProjectileAttackAbility : AttackAbility
{
    public GameObject Projectile;

    public override void ActivateAbility(AbilityInstance abilityInstance)
    {
        var asc = abilityInstance.AbilitySystemComponent;
        var attacker = asc.GetComponent<Creature>();

        GetDefaultAttackOriginAndDirection(abilityInstance, out var origin, out var direction);

        var projectileGameObject = Instantiate(Projectile, origin, Quaternion.LookRotation(direction, Vector3.up));
        var projectile = projectileGameObject.GetComponent<Projectile>();

        projectile.Owner = asc.gameObject;

        // TODO: damage, range etc.

        abilityInstance.End();
    }
}