using UnityEngine;

[AbilityInstanceDataClass]
public class AbsorbAbilityInstanceData
{

}

[CreateAssetMenu(menuName = "Abilities/Absorb")]
public class AbsorbAbility : Ability
{
    public AttributeSet AbilityAttributeSet;
    public LayerMask LayerMask;

    [SerializeField] private float absorptionSpeed = 0f;
    [SerializeField] private float maxAbsorptionSpeed = 13f;
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
        var data = abilityInstance.Data as AbsorbAbilityInstanceData;

        Debug.Assert(data != null);

        var asc = abilityInstance.AbilitySystemComponent;

        // TODO: cache attribute value objects

        var aoeRangeModifier = ScalarModifier.MakeIdentity();
        
        aoeRangeModifier.Combine(ScalarModifier.MakeBonus(asc.GetAttributeValue(AttributeType.AreaOfEffectBonus)));
        aoeRangeModifier.Combine(ScalarModifier.MakeBonusFraction(asc.GetAttributeValue(AttributeType.AreaOfEffectBonusFraction)));
        aoeRangeModifier.Combine(ScalarModifier.MakeMultiplier(asc.GetAttributeValue(AttributeType.AreaOfEffectMultiplier)));

        var absorbRadius = asc.GetAttributeValue(AttributeType.AbsorptionRadius);

        Absorb(abilityInstance, aoeRangeModifier.Calculate(absorbRadius));
    }

    private void Absorb(AbilityInstance abilityInstance, float range)
    {
        var asc = abilityInstance.AbilitySystemComponent; 
        var ascAvatar = asc.GetComponent<Creature>(); //Player instance

        var colliders = Physics.OverlapSphere(asc.transform.position, range, LayerMask);

        foreach (var collider in colliders)
        {
            var pickup = Pickup.GetPickupGameObject(collider);
            if (pickup == null)
                continue;

            //Non-linear movement no longer works. Need a data structure of some sorts
            // absorptionSpeed += absorptionAcceleration * Time.deltaTime;
            // //Set a cap on the max speed of the orb
            // absorptionSpeed = Mathf.Min(absorptionSpeed, maxAbsorptionSpeed);

            pickup.transform.position = Vector3.MoveTowards(pickup.transform.position, ascAvatar.transform.position, absorptionSpeed * Time.deltaTime);
        }
    }
}
