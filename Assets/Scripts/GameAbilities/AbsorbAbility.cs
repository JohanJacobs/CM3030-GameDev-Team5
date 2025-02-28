/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

AbsorbAbility.cs

*/

using UnityEngine;
using System.Collections.Generic;

[AbilityInstanceDataClass]
public class AbsorbAbilityInstanceData : AbilityInstanceData
{
    //Dictionary of orbs and their individual speeds
    public Dictionary<GameObject, float> OrbSpeeds = new Dictionary<GameObject, float>();

    public void Reset()
    {
        OrbSpeeds.Clear();
    }
}

[CreateAssetMenu(menuName = "Abilities/Absorb")]
public class AbsorbAbility : Ability
{
    public LayerMask LayerMask;

    public Magnitude Range = new Magnitude()
    {
        Calculation = MagnitudeCalculation.AttributeAdd,
        Attribute = AttributeType.AttackRange,
        Value = 4,
    };

    [SerializeField] private float absorptionSpeed = 0.5f;
    [SerializeField] private float maxAbsorptionSpeed = 14f;
    [SerializeField] private float absorptionAcceleration = 3f;

    public AbsorbAbility()
    {
        ActivateOnGranted = true;
        AbilityInstanceDataClass = new AbilityInstanceDataClass(typeof(AbsorbAbilityInstanceData));
    }

    public override void UpdateAbility(AbilityInstance abilityInstance, float deltaTime)
    {
        var data = abilityInstance.GetData<AbsorbAbilityInstanceData>();

        // NOTE: this could be a clone with different properties so we're using the one AbilityInstance was created for
        var ability = abilityInstance.GetAbility<AbsorbAbility>();

        var range = abilityInstance.CalculateMagnitude(ability.Range);

        Debug.Assert(range > 0);

        AbsorbPickupsInRange(data, abilityInstance.Owner.transform.position, range, ability.LayerMask, deltaTime);
    }

    public override void ActivateAbility(AbilityInstance abilityInstance)
    {
        var data = abilityInstance.GetData<AbsorbAbilityInstanceData>();

        data.Reset();
    }

    public override void EndAbility(AbilityInstance abilityInstance)
    {
        var data = abilityInstance.GetData<AbsorbAbilityInstanceData>();

        data.Reset();
    }

    private void AbsorbPickupsInRange(AbsorbAbilityInstanceData data, Vector3 origin, float range, LayerMask layerMask, float deltaTime)
    {
        var targetQuery = new AbilityTargetQuery()
        {
            LayerMask = layerMask,
            Origin = origin,
            Range = range,
        };

        var targets = AbilityTargetSelector.GetAreaTargets(targetQuery);

        foreach (var target in targets)
        {
            var pickup = Pickup.GetPickupGameObject(target.Collider);
            if (pickup == null)
                continue;

            if (!data.OrbSpeeds.TryGetValue(pickup, out var speed))
            {
                speed = absorptionSpeed;
            }

            //Move orb towards player
            pickup.transform.position = Vector3.MoveTowards(pickup.transform.position, origin, speed * deltaTime);

            //Update speed for this particular orb
            speed = Mathf.Min(speed + absorptionAcceleration * deltaTime, maxAbsorptionSpeed);

            data.OrbSpeeds[pickup] = speed;

            //LINEAR ACCELERATION
            // pickup.transform.position = Vector3.MoveTowards(pickup.transform.position, origin, absorptionSpeed * deltaTime);
        }
    }
}
