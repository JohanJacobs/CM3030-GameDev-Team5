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
        // TODO: consider OverlapSphereNonAlloc
        // TODO: could be too expensive to trigger every update
        var colliders = Physics.OverlapSphere(origin, range, layerMask);

        foreach (var collider in colliders)
        {
            var pickup = Pickup.GetPickupGameObject(collider);
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
            //pickup.transform.position = Vector3.MoveTowards(pickup.transform.position, origin, absorptionSpeed * deltaTime);
        }
    }
}