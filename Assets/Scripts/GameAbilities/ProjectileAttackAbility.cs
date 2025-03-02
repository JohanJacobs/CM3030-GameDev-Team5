/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

ProjectileAttackAbility.cs

*/

using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

// TODO: add more types?
public enum ProjectileAttackLaunch
{
    None,
    Drop,
    ThrowAtTarget,
    ThrowAlongAimDirection,
}

[CreateAssetMenu(menuName = "Abilities/Projectile Attack")]
public class ProjectileAttackAbility : AttackAbility
{
    public GameObject Projectile;
    public ProjectileAttackLaunch Launch = ProjectileAttackLaunch.None;
    /// <summary>
    /// Used to throw projectile towards target
    /// </summary>
    public Magnitude ThrowSpeed;

    public override void ActivateAbility(AbilityInstance abilityInstance)
    {
        Vector3 origin;
        Vector3 direction;

        GetEquipmentAimWithOwnerDirection(abilityInstance, out origin, out direction);

        var projectileGameObject = Instantiate(Projectile, origin, Quaternion.LookRotation(direction, Vector3.up));
        var projectile = projectileGameObject.GetComponent<Projectile>();

        projectile.Initialize(abilityInstance);

        switch (Launch)
        {
            case ProjectileAttackLaunch.None:
                break;
            case ProjectileAttackLaunch.Drop:
                DropProjectile(abilityInstance, projectile);
                break;
            case ProjectileAttackLaunch.ThrowAtTarget:
                ThrowProjectileAtTarget(abilityInstance, projectile);
                break;
            case ProjectileAttackLaunch.ThrowAlongAimDirection:
                ThrowProjectile(abilityInstance, projectile);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        abilityInstance.End();
    }

    /// <summary>
    /// Launches projectile so that it lands near aim target. Solves for speed with fixed launch angle.
    /// </summary>
    /// <remarks>
    /// Assumes gravity is straight-down. Does not account for drag.
    /// </remarks>
    /// <param name="abilityInstance"></param>
    /// <param name="projectile"></param>
    private void ThrowProjectileAtTarget(AbilityInstance abilityInstance, Projectile projectile)
    {
        // adopted from https://discussions.unity.com/t/how-to-calculate-force-needed-to-jump-towards-target-point/607902/6

        // TODO: consider ability range

        const float launchAngle = 30f;
        const float launchAngleRadians = launchAngle * Mathf.Deg2Rad;

        var origin = projectile.transform.position;

        GetOwnerAimTarget(abilityInstance, out var target);

        Vector3 xzOrigin = new Vector3(origin.x, 0, origin.z);
        Vector3 xzTarget = new Vector3(target.x, 0, target.z);

        Vector3 xzDelta = xzTarget - xzOrigin;

        float xzDistance = xzDelta.magnitude;
        float yDistance = target.y - origin.y;

        float gravity = Physics.gravity.magnitude;
        float speed = (1f / Mathf.Cos(launchAngleRadians)) * Mathf.Sqrt((0.5f * gravity * xzDistance * xzDistance) / (xzDistance * Mathf.Tan(launchAngleRadians) - yDistance));

        Vector3 xzDirection = xzDelta.normalized;
        Vector3 yDirection = Vector3.up;

        Vector3 velocity = xzDirection * speed * Mathf.Sin(launchAngleRadians) + yDirection * speed * Mathf.Cos(launchAngleRadians);

        projectile.ProjectileRigidBody.MoveRotation(Quaternion.LookRotation(velocity, yDirection));
        projectile.ProjectileRigidBody.AddForce(velocity, ForceMode.VelocityChange);

        // NOTE: adding some rotation makes thrown projectiles less boring
        projectile.ProjectileRigidBody.AddRelativeTorque(Random.Range(3f, 5f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), ForceMode.VelocityChange);
    }

    private void ThrowProjectile(AbilityInstance abilityInstance, Projectile projectile)
    {
        // TODO: consider ability range

        const float launchAngle = 15f;
        const float launchAngleRadians = launchAngle * Mathf.Deg2Rad;

        var speed = abilityInstance.CalculateMagnitude(ThrowSpeed);

        Vector3 xzDirection = projectile.transform.forward;

        xzDirection.y = 0;
        xzDirection.Normalize();

        Vector3 yDirection = Vector3.up;

        Vector3 velocity = xzDirection * speed * Mathf.Sin(launchAngleRadians) + yDirection * speed * Mathf.Cos(launchAngleRadians);

        projectile.ProjectileRigidBody.MoveRotation(Quaternion.LookRotation(velocity, yDirection));
        projectile.ProjectileRigidBody.AddForce(velocity, ForceMode.VelocityChange);

        // NOTE: adding some rotation makes thrown projectiles less boring
        projectile.ProjectileRigidBody.AddRelativeTorque(Random.Range(3f, 5f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), ForceMode.VelocityChange);
    }

    private void DropProjectile(AbilityInstance abilityInstance, Projectile projectile)
    {
        projectile.ProjectileRigidBody.AddForce(projectile.transform.forward, ForceMode.VelocityChange);

        // NOTE: adding some rotation makes thrown projectiles less boring
        projectile.ProjectileRigidBody.AddRelativeTorque(Random.Range(3f, 5f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), ForceMode.VelocityChange);
    }
}
