/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

ProjectileAttackAbility.cs

*/

using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Projectile Attack")]
public class ProjectileAttackAbility : AttackAbility
{
    public GameObject Projectile;

    public override void ActivateAbility(AbilityInstance abilityInstance)
    {
        Vector3 origin;
        Vector3 direction;

        if (GetEquipmentAim(abilityInstance, out origin, out direction))
        {
        }
        else
        {
            GetOwnerAim(abilityInstance, out origin, out direction);
        }

        var asc = abilityInstance.AbilitySystemComponent;

        var projectileGameObject = Instantiate(Projectile, origin, Quaternion.LookRotation(direction, Vector3.up));
        var projectile = projectileGameObject.GetComponent<Projectile>();

        projectile.Owner = asc.Owner.gameObject;

        NotifyAttackCommitted(abilityInstance, origin, direction, 0);

        abilityInstance.End();
    }
}
