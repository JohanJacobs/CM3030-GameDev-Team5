using UnityEngine;
using System.Collections.Generic;

[AbilityInstanceDataClass]
public class AbsorbAbilityInstanceData : AbilityInstanceData
{
    //Dictionary of orbs and their individual speeds
    public Dictionary<GameObject, float> OrbSpeeds = new Dictionary<GameObject, float>();
}

[CreateAssetMenu(menuName = "Abilities/Absorb")]
public class AbsorbAbility : Ability
{
    public AttributeSet AbilityAttributeSet;
    public LayerMask LayerMask;

    [SerializeField] private float absorptionSpeed = 0.5f;
    [SerializeField] private float maxAbsorptionSpeed = 14f;
    [SerializeField] private float absorptionAcceleration = 3f;

    public AbsorbAbility()
    {
        ActivateOnGranted = true;
        AbilityInstanceDataClass = new AbilityInstanceDataClass(typeof(AbsorbAbilityInstanceData));
    }

    public override void HandleAbilityAdded(AbilityInstance abilityInstance)
    {
        abilityInstance.AbilitySystemComponent.AddAttributeSet(AbilityAttributeSet);
    }

    public override void HandleAbilityRemoved(AbilityInstance abilityInstance)
    {
        abilityInstance.AbilitySystemComponent.RemoveAttributeSet(AbilityAttributeSet);
    }

    public override void UpdateAbility(AbilityInstance abilityInstance, float deltaTime)
    {
        var asc = abilityInstance.AbilitySystemComponent;

        // TODO: cache attribute value objects

        var aoeRangeModifier = ScalarModifier.MakeIdentity();

        aoeRangeModifier.Combine(ScalarModifier.MakeBonus(asc.GetAttributeValue(AttributeType.AreaOfEffectBonus)));
        aoeRangeModifier.Combine(ScalarModifier.MakeBonusFraction(asc.GetAttributeValue(AttributeType.AreaOfEffectBonusFraction)));
        aoeRangeModifier.Combine(ScalarModifier.MakeMultiplier(asc.GetAttributeValue(AttributeType.AreaOfEffectMultiplier)));

        var absorbRadius = asc.GetAttributeValue(AttributeType.AbsorptionRadius);

        Absorb(abilityInstance, aoeRangeModifier.Calculate(absorbRadius));
    }

    public override void ActivateAbility(AbilityInstance abilityInstance)
    {
        var data = abilityInstance.GetData<AbsorbAbilityInstanceData>();

        data.OrbSpeeds.Clear();
    }

    public override void EndAbility(AbilityInstance abilityInstance)
    {
        var data = abilityInstance.GetData<AbsorbAbilityInstanceData>();

        data.OrbSpeeds.Clear();
    }

    private void Absorb(AbilityInstance abilityInstance, float range)
    {
        var asc = abilityInstance.AbilitySystemComponent;
        var ascAvatar = asc.GetComponent<Creature>(); //Player instance
        var data = abilityInstance.GetData<AbsorbAbilityInstanceData>(); //For orb tracking

        var colliders = Physics.OverlapSphere(asc.transform.position, range, LayerMask);

        foreach (var collider in colliders)
        {
            var pickup = Pickup.GetPickupGameObject(collider);
            if (pickup == null)
                continue;

            if (!data.OrbSpeeds.ContainsKey(pickup))
            {
                data.OrbSpeeds[pickup] = absorptionSpeed;
            }

            //Update speed for this particular orb
            data.OrbSpeeds[pickup] += absorptionAcceleration * Time.deltaTime;
            data.OrbSpeeds[pickup] = Mathf.Min(data.OrbSpeeds[pickup], maxAbsorptionSpeed);
            //Move orb towards player
            pickup.transform.position = Vector3.MoveTowards(pickup.transform.position, ascAvatar.transform.position, data.OrbSpeeds[pickup] * Time.deltaTime);


            //LINEAR ACCELERATION
            //pickup.transform.position = Vector3.MoveTowards(pickup.transform.position, ascAvatar.transform.position, absorptionSpeed * Time.deltaTime);
        }
    }
}