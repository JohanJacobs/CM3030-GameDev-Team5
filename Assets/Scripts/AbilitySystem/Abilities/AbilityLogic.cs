using System;

public abstract class AbilityLogic
{
    public virtual void ActivateAbility(AbilityInstance abilityInstance)
    {

    }

    public virtual bool CanActivateAbility(AbilityInstance abilityInstance)
    {
        return true;
    }

    public virtual bool CommitAbility(AbilityInstance abilityInstance)
    {
        return CanActivateAbility(abilityInstance);
    }

    public virtual void EndAbility(AbilityInstance abilityInstance)
    {

    }

    public virtual void UpdateAbility(AbilityInstance abilityInstance, float deltaTime)
    {

    }
}

public sealed class NullAbilityLogic : AbilityLogic
{

}

[AbilityLogicClass]
public class TestAbilityLogic : AbilityLogic
{
    public override void UpdateAbility(AbilityInstance abilityInstance, float deltaTime)
    {
    }
}
