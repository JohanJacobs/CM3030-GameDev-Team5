using UnityEngine;

[AbilityInstanceDataClass]
public class GarlicAbilityInstanceData
{
    public float TimeToNextAttack;
}

[CreateAssetMenu(menuName = "Abilities/Garlic")]
public class GarlicAbility : Ability
{
    public Effect TargetEffect;
    public AttributeSet AbilityAttributeSet;
    public LayerMask LayerMask;

    public GarlicAbility()
    {
        ActivateOnGranted = true;
        AbilityInstanceDataClass = new AbilityInstanceDataClass(typeof(GarlicAbilityInstanceData));
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
        var data = abilityInstance.Data as GarlicAbilityInstanceData;

        Debug.Assert(data != null);

        data.TimeToNextAttack -= deltaTime;

        if (data.TimeToNextAttack > 0)
            return;

        var asc = abilityInstance.AbilitySystemComponent;

        // TODO: cache attribute value objects

        var aoeRangeModifier = ScalarModifier.MakeIdentity();

        aoeRangeModifier.Combine(ScalarModifier.MakeBonus(asc.GetAttributeValue(AttributeType.AreaOfEffectBonus)));
        aoeRangeModifier.Combine(ScalarModifier.MakeBonusFraction(asc.GetAttributeValue(AttributeType.AreaOfEffectBonusFraction)));
        aoeRangeModifier.Combine(ScalarModifier.MakeMultiplier(asc.GetAttributeValue(AttributeType.AreaOfEffectMultiplier)));

        var attackRange = asc.GetAttributeValue(AttributeType.GarlicAttackRange);
        var attackRate = asc.GetAttributeValue(AttributeType.GarlicAttackRate);
        var attackDamage = asc.GetAttributeValue(AttributeType.GarlicAttackDamage);

        data.TimeToNextAttack += 1f / attackRate;

        Attack(abilityInstance, aoeRangeModifier.Calculate(attackRange), attackDamage);
    }

    private void Attack(AbilityInstance abilityInstance, float range, float damage)
    {
        var asc = abilityInstance.AbilitySystemComponent;

        var colliders = Physics.OverlapSphere(asc.transform.position, range, LayerMask);

        foreach (var collider in colliders)
        {
            var monster = collider.GetComponentInParent<Monster>();
            if (monster == null)
                continue;

            if (TargetEffect != null)
            {
                asc.ApplyEffectToTarget(TargetEffect, monster.AbilitySystemComponent);
            }

            monster.TakeDamage(asc.gameObject, asc.transform.position, damage);
        }
    }
}